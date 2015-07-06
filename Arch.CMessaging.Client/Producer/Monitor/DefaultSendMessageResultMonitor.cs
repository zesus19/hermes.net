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

namespace Arch.CMessaging.Client.Producer.Monitor
{
    [Named(ServiceType = typeof(ISendMessageResultMonitor))]
    public class DefaultSendMessageResultMonitor : ISendMessageResultMonitor, IInitializable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DefaultSendMessageResultMonitor));
        private Dictionary<long, SendMessageCommand> commands = new Dictionary<long, SendMessageCommand>();
        private object syncRoot = new object();
        private Timer timer;

        [Inject]
        private ISystemClockService systemClockService;

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
                        commands.Remove(correlationId);
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
                lock (syncRoot)
                {
                    foreach (var entry in commands)
                    {
                        SendMessageCommand command = entry.Value;
                        var correlationId = entry.Key;
                        if (command.ExpireTime < systemClockService.Now())
                        {
                            commands.Remove(correlationId);
                        }
                    }
                }
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
    }
}
