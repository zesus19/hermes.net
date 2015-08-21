using System;
using Freeway.Logging;
using Arch.CMessaging.Client.Core.Service;
using System.Collections.Concurrent;
using Arch.CMessaging.Client.Core.Ioc;
using Arch.CMessaging.Client.Transport.Command;
using System.Threading;
using System.Collections.Generic;

namespace Arch.CMessaging.Client.Consumer.Engine.Monitor
{
    [Named(ServiceType = typeof(IPullMessageResultMonitor))]
    public class DefaultPullMessageResultMonitor : IPullMessageResultMonitor
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DefaultPullMessageResultMonitor));

        [Inject]
        private ISystemClockService systemClockService;

        private ConcurrentDictionary<long, PullMessageCommand> cmds = new ConcurrentDictionary<long, PullMessageCommand>();

        public void Monitor(PullMessageCommand cmd)
        {
            if (cmd != null)
            {
                cmds[cmd.Header.CorrelationId] = cmd;
            }
        }

        public void ResultReceived(PullMessageResultCommand result)
        {
            if (result != null)
            {
                PullMessageCommand pullMessageCommand = null;
                cmds.TryRemove(result.Header.CorrelationId, out pullMessageCommand);

                if (pullMessageCommand != null)
                {
                    try
                    {
                        pullMessageCommand.OnResultReceived(result);
                    }
                    catch (Exception e)
                    {
                        log.Warn("Exception occurred while calling resultReceived", e);
                    }
                }
                else
                {
                    result.Release();
                }
            }
        }

        public void Remove(PullMessageCommand cmd)
        {
            PullMessageCommand removedCmd = null;
            if (cmd != null)
            {
                cmds.TryRemove(cmd.Header.CorrelationId, out removedCmd);
            }
        }

    }
}

