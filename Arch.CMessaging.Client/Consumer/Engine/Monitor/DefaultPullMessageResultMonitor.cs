using System;
using Freeway.Logging;
using Arch.CMessaging.Client.Core.Service;
using System.Collections.Concurrent;
using Arch.CMessaging.Client.Core.Ioc;
using Arch.CMessaging.Client.Transport.Command;

namespace Arch.CMessaging.Client.Consumer.Engine.Monitor
{
    public class DefaultPullMessageResultMonitor : IPullMessageResultMonitor, IInitializable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DefaultPullMessageResultMonitor));

        [Inject]
        private ISystemClockService m_systemClockService;

        private ConcurrentDictionary<long, PullMessageCommand> m_cmds = new ConcurrentDictionary<long, PullMessageCommand>();

        private object theLock = new object();

        public void monitor(PullMessageCommand cmd)
        {
            if (cmd != null)
            {
                lock (theLock)
                {
                    m_cmds.TryAdd(cmd.Header.CorrelationId, cmd);
                }
            }
        }

        public void resultReceived(PullMessageResultCommand result)
        {
            if (result != null)
            {
                PullMessageCommand pullMessageCommand = null;
                lock (theLock)
                {
                    m_cmds.TryRemove(result.Header.CorrelationId, out pullMessageCommand);
                }
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

        public void Initialize()
        {
            /*
            Executors.newSingleThreadScheduledExecutor(
                HermesThreadFactory.create("PullMessageResultMonitor-HouseKeeper", true)).scheduleWithFixedDelay(
                    new Runnable() {

                    @Override
                    public void run() {
                        try {
                            List<PullMessageCommand> timeoutCmds = new LinkedList<>();

                            m_lock.lock();
                            try {
                                for (Map.Entry<Long, PullMessageCommand> entry : m_cmds.entrySet()) {
                                    PullMessageCommand cmd = entry.getValue();
                                    Long correlationId = entry.getKey();
                                    if (cmd.getExpireTime() + 4000L < m_systemClockService.now()) {
                                        timeoutCmds.add(m_cmds.remove(correlationId));
                                    }
                                }

                            } finally {
                                m_lock.unlock();
                            }

                            for (PullMessageCommand timeoutCmd : timeoutCmds) {
                                if (log.isDebugEnabled()) {
                                    log.debug(
                                        "No result received for PullMessageCommand(correlationId={}) until timeout, will cancel waiting automatically",
                                        timeoutCmd.getHeader().getCorrelationId());
                                }
                                timeoutCmd.onTimeout();
                            }
                        } catch (Exception e) {
                            // ignore
                            if (log.isDebugEnabled()) {
                                log.debug("Exception occurred while running PullMessageResultMonitor-HouseKeeper", e);
                            }
                        }
                    }
                }, 5, 5, TimeUnit.SECONDS);
                */
        }
    }
}

