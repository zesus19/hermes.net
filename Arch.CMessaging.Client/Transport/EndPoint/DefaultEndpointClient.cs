using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using Arch.CMessaging.Client.MetaEntity.Entity;
using Arch.CMessaging.Client.Transport.Command;
using Freeway.Logging;
using Arch.CMessaging.Client.Core.Config;
using Arch.CMessaging.Client.Transport.Command.Processor;
using Arch.CMessaging.Client.Core.Service;
using Arch.CMessaging.Client.Net.Core.Future;
using Arch.CMessaging.Client.Core.Utils;
using Arch.CMessaging.Client.Core.Collections;
using Arch.CMessaging.Client.Net.Core.Session;
using Arch.CMessaging.Client.Net;
using Arch.CMessaging.Client.Net.Filter.Codec;
using System.Net;
using System.Threading;
using Arch.CMessaging.Client.Core.Ioc;
using System.Diagnostics;
using Arch.CMessaging.Client.Core.Schedule;

namespace Arch.CMessaging.Client.Transport.EndPoint
{
	[Named (ServiceType = typeof(IEndpointClient))]
	public class DefaultEndpointClient : IEndpointClient, IInitializable
	{
		[Inject]
		private CoreConfig config;
		[Inject]
		private CommandProcessorManager commandProcessorManager;
		[Inject]
		private ISystemClockService systemClockService;

		private Timer timer;
        private ISchedulePolicy schedulePolicy;
		private object syncRoot = new object ();
		private ConcurrentDictionary<Endpoint, EndpointSession> sessions;
		private static readonly ILog log = LogManager.GetLogger (typeof(DefaultEndpointClient));

        public DefaultEndpointClient()
        {
            this.sessions = new ConcurrentDictionary<Endpoint, EndpointSession>();
        }

		#region IEndpointClient Members

		public CoreConfig Config { get { return config; } }

		public ISystemClockService ClockService { get { return systemClockService; } }

		public ILog Log { get { return log; } }

		public void WriteCommand (Endpoint endpoint, ICommand command)
		{
			WriteCommand (endpoint, command, config.EndpointSessionDefaultWrtieTimeoutInMills);
		}

		public void WriteCommand (Endpoint endpoint, ICommand command, int timeoutInMills)
		{
			GetSession (endpoint).Write (command, timeoutInMills);
		}

		#endregion

		public void Initialize ()
		{
            int checkBase = config.EndpointSessionWriterCheckIntervalBase;
            int checkMax = config.EndpointSessionWriterCheckIntervalMax;
            schedulePolicy = new ExponentialSchedulePolicy(checkBase, checkMax);

            timer = new Timer (EndpointSessionFlush, null, checkBase, Timeout.Infinite);
		}

		private EndpointSession GetSession (Endpoint endpoint)
		{
			switch (endpoint.Type) {
			case Endpoint.BROKER:
				{
					EndpointSession session = null;
					if (!sessions.TryGetValue (endpoint, out session)) {
						lock (syncRoot) {
							if (!sessions.TryGetValue (endpoint, out session)) {
                                session = CreateSession (endpoint);
                                sessions[endpoint] = session;
							}
						}
					}
					return session;
				}
			default:
				throw new ArgumentException (string.Format ("Unknow endpoint type: {0}", endpoint.Type));
			}
		}

		private EndpointSession CreateSession (Endpoint endpoint)
		{
			var session = new EndpointSession (this);
			Connect (endpoint, session);
			return session;
		}

		private void Connect (Endpoint endpoint, EndpointSession endpointSession)
		{
            Debug.WriteLine("producer try connect {0}:{1}", endpoint.Host, endpoint.Port);
			var future = CreateBootstrap (endpoint, endpointSession)
                .Connect (new IPEndPoint (IPAddress.Parse (endpoint.Host), endpoint.Port));
			future.Complete += (s, e) => {
				var connectFuture = e.Future as DefaultConnectFuture;
				if (!endpointSession.IsClosed) {
					if (!connectFuture.Connected) {
                        Log.Error(string.Format("producer try connect failed at {0}:{1}", endpoint.Host, endpoint.Port));
						endpointSession.SetSessionFuture (null);
						System.Threading.Thread.Sleep (config.EndpointSessionAutoReconnectDelay * 1000);
                        Debug.WriteLine("producer try connect failed at {0}:{1}", endpoint.Host, endpoint.Port);
						Connect (endpoint, endpointSession);
					} else
						endpointSession.SetSessionFuture (connectFuture);
				} else {
					if (connectFuture.Connected)
						future.Session.Close (true);
				}
			};
		}

		public void RemoveSession (Endpoint endpoint, EndpointSession endpointSession)
		{
			EndpointSession removedSession = null;
            if (Endpoint.BROKER.Equals(endpoint.Type) && sessions.ContainsKey(endpoint))
            {
                lock (syncRoot)
                {
                    if (sessions.ContainsKey(endpoint))
                    {
                        EndpointSession tmp = sessions[endpoint];
                        if (tmp == endpointSession)
                        {
                            if (tmp.IsClosed)
                                sessions.TryRemove(endpoint, out removedSession);
                            else if (!tmp.IsFlushing && !tmp.HasUnflushOps)
                                sessions.TryRemove(endpoint, out removedSession);
                        }
                    }
                }
            }
            if (removedSession != null)
            {
                log.Info(string.Format("Closing idle connection to broker({0}:{1})", endpoint.Host, endpoint.Port));
                removedSession.Close();
            }
		}

		private Bootstrap CreateBootstrap (Endpoint endpoint, EndpointSession endpointSession)
		{
            return new Bootstrap()
                    .Option(SessionOption.SO_KEEPALIVE, true)
                    .Option(SessionOption.CONNECT_TIMEOUT_MILLIS, 5000)
                    .Option(SessionOption.TCP_NODELAY, true)
                    .Option(SessionOption.SO_SNDBUF, config.SendBufferSize)
                    .Option(SessionOption.SO_RCVBUF, config.ReceiveBufferSize)
                    .Option(SessionOption.BOTH_IDLE_TIME, config.EndpointSessionMaxIdleTime)
                    .OnSessionDestroyed((session) => this.RemoveSession(endpoint, endpointSession))
                    .Handler(chain =>
                    {
                        chain.AddLast(
                            new ExceptionHandler(),
                            new MagicNumberPrepender(),
                            new LengthFieldPrepender(4),
                            new ProtocolCodecFilter(new CommandCodecFactory()));
                    })
                    .Handler(new DefaultClientChannelInboundHandler(commandProcessorManager, endpoint, endpointSession, this, config));
		}

        private void EndpointSessionFlush(object state)
        {
            timer.Change(Timeout.Infinite, Timeout.Infinite);
            bool flushed = false;
            try
            {
                foreach (var session in sessions.Values)
                {
                    if (!session.IsClosed)
                        flushed = flushed || session.Flush();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            finally
            {
                if (flushed)
                {
                    schedulePolicy.Succeess();
                }
                timer.Change(schedulePolicy.Fail(false), Timeout.Infinite);
            }
        }
	}
}
