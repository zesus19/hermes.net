using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Arch.CMessaging.Client.Core.Ioc;
using Arch.CMessaging.Client.Core.Service;
using Arch.CMessaging.Client.Transport.Command;
using Freeway.Logging;
using Arch.CMessaging.Client.Producer.Config;
using System.Collections.Concurrent;
using Arch.CMessaging.Client.Core.Message;
using Arch.CMessaging.Client.Core.Future;
using Arch.CMessaging.Client.Core.Result;
using Arch.CMessaging.Client.Core.Utils;
using Arch.CMessaging.Client.Producer.Sender;

namespace Arch.CMessaging.Client.Producer.Monitor
{
    [Named(ServiceType = typeof(ISendMessageResultMonitor))]
    public class DefaultSendMessageResultMonitor : ISendMessageResultMonitor, IInitializable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DefaultSendMessageResultMonitor));
        private ConcurrentDictionary<long, SendMessageCommand> commands = new ConcurrentDictionary<long, SendMessageCommand>();
        private object syncRoot = new object();
        private Timer timer;

        [Inject]
        private ISystemClockService systemClockService;

        [Inject]
        private ProducerConfig config;

        #region ISendMessageResultMonitor Members

        public void Monitor(SendMessageCommand command)
        {
            if (command != null)
            {
                lock (syncRoot)
                {
                    commands[command.Header.CorrelationId] = command;
                }
            }
        }

        public void ResultReceived(SendMessageResultCommand result)
        {
            if (result != null)
            {
                SendMessageCommand sendMessageCommand = null;
                lock (syncRoot)
                {
                    var correlationId = result.Header.CorrelationId;
                    if (commands.TryGetValue(correlationId, out sendMessageCommand))
                    {
                        SendMessageCommand oldCommand;
                        commands.TryRemove(correlationId, out oldCommand);
                    }
                }
                if (sendMessageCommand != null)
                {
                    try
                    {
                        sendMessageCommand.OnResultReceived(result);
                        Tracking(sendMessageCommand, true);
                    }
                    catch (Exception ex)
                    {
                        log.Warn(ex);
                    }
                }
            }
        }

        #endregion

        private void Tracking(SendMessageCommand sendMessageCommand, bool success)
        {
            //todo
        }

        #region IInitializable Members

        public void Initialize()
        {
            timer = new Timer(TimeoutCheck, null, 5, Timeout.Infinite);
        }

        #endregion

        private void TimeoutCheck(object state)
        {
            timer.Change(Timeout.Infinite, Timeout.Infinite);
            try
            {
                ScanAndResendTimeoutCommands();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            finally
            {
                timer.Change(5, Timeout.Infinite);
            }
        }

        protected void ScanAndResendTimeoutCommands()
        {
            List<SendMessageCommand> timeoutCmds = ScanTimeoutCommands();

            if (timeoutCmds.Count != 0)
            {
                resend(timeoutCmds);
            }
        }

        protected List<SendMessageCommand> ScanTimeoutCommands()
        {
            List<SendMessageCommand> timeoutCmds = new List<SendMessageCommand>();
            lock (syncRoot)
            {
                foreach (KeyValuePair<long, SendMessageCommand> entry in commands)
                {
                    SendMessageCommand cmd = entry.Value;
                    long correlationId = entry.Key;
                    if (cmd.isExpired(systemClockService.Now(), config.SendMessageReadResultTimeoutMillis))
                    {
                        SendMessageCommand oldCmd;
                        commands.TryRemove(correlationId, out oldCmd);
                        timeoutCmds.Add(cmd);
                    }
                }
            }
            return timeoutCmds;
        }

        protected void resend(List<SendMessageCommand> timeoutCmds)
        {
            foreach (SendMessageCommand cmd in timeoutCmds)
            {
                List<Pair<ProducerMessage, SettableFuture<SendResult>>> msgFuturePairs = cmd
                    .GetProducerMessageFuturePairs();
                foreach (Pair<ProducerMessage, SettableFuture<SendResult>> pair in msgFuturePairs)
                {
                    IMessageSender sender = ComponentLocator.Lookup<IMessageSender>();
                    if (sender != null)
                    {
                        sender.Resend(pair.Key, pair.Value);
                    }
                }
            }
        }
    }
}
