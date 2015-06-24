using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using Arch.CMessaging.Client.Meta.Entity;
using Arch.CMessaging.Client.Transport.Command;
using Freeway.Logging;
using Arch.CMessaging.Client.Core.Config;
using Arch.CMessaging.Client.Transport.Command.Processor;
using Arch.CMessaging.Client.Core.Service;

namespace Arch.CMessaging.Client.Transport.EndPoint
{
    public class DefaultEndpointClient : IEndpointClient
    {
        //ioc inject
        private CoreConfig config;
        //ioc inject
        private CommandProcessorManager commandProcessorManager;
        //ioc inject
        private ISystemClockService systemClockService;
        private object syncRoot = new object();
        private ConcurrentDictionary<Endpoint, EndpointSession> sessions;
        private static readonly ILog log = LogManager.GetLogger(typeof(DefaultEndpointClient));
        #region IEndpointClient Members

        public void WriteCommand(Endpoint endpoint, ICommand command)
        {
            WriteCommand(endpoint, command, config.EndpointSessionDefaultWrtieTimeout);
        }

        public void WriteCommand(Endpoint endpoint, ICommand command, int timeoutInMills)
        {
           
        }

        #endregion

        private EndpointSession GetSession(Endpoint endpoint)
        {
            switch (endpoint.Type)
            {
                case Endpoint.BROKER:
                    {
                        EndpointSession session = null;
                        if (!sessions.TryGetValue(endpoint, out session))
                        {
                            lock (syncRoot)
                            {
                                if (!sessions.TryGetValue(endpoint, out session))
                                {
                                    sessions[endpoint] = CreateSession(endpoint);
                                }
                            }
                        }
                        return session;
                    }
                default: throw new ArgumentException(string.Format("Unknow endpoint type: {0}", endpoint.Type));
            }
        }

        private EndpointSession CreateSession(Endpoint endpoint)
        {
            return null;
        }

        private void Connect(Endpoint endpoint, EndpointSession endpointSession)
        {
 
        }
    }
}
