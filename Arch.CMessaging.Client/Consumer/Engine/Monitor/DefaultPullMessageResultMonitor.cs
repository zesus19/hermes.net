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
    public class DefaultPullMessageResultMonitor : IPullMessageResultMonitor, IInitializable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DefaultPullMessageResultMonitor));

        [Inject]
        private ISystemClockService systemClockService;

        private ConcurrentDictionary<long, PullMessageCommand> cmds = new ConcurrentDictionary<long, PullMessageCommand>();

        private object theLock = new object();

        private Timer timer;

        public void Monitor(PullMessageCommand cmd)
        {
            if (cmd != null)
            {
                lock (theLock)
                {
                    cmds[cmd.Header.CorrelationId] = cmd;
                }
            }
        }

        public void ResultReceived(PullMessageResultCommand result)
        {
            if (result != null)
            {
                PullMessageCommand pullMessageCommand = null;
                try
                {
                    lock (theLock)
                    {
                        cmds.TryRemove(result.Header.CorrelationId, out pullMessageCommand);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
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
            timer = new Timer(HouseKeep, null, 5000, Timeout.Infinite);
        }

        private void HouseKeep(object dummy)
        {
            timer.Change(Timeout.Infinite, Timeout.Infinite);
            try
            {
                List<PullMessageCommand> timeoutCmds = new List<PullMessageCommand>();

                lock (theLock)
                {
                    foreach (KeyValuePair<long, PullMessageCommand> entry in cmds)
                    {
                        PullMessageCommand cmd = entry.Value;
                        long correlationId = entry.Key;
                        if (cmd.ExpireTime + 4000L < systemClockService.Now())
                        {
                            PullMessageCommand foo;
                            cmds.TryRemove(correlationId, out foo);
                            timeoutCmds.Add(cmd);
                        }
                    }

                } 

                foreach (PullMessageCommand timeoutCmd in timeoutCmds)
                {
                    timeoutCmd.OnTimeout();
                }
            }
            catch
            {
                //ignore
            }
            finally
            {
                timer.Change(5000, Timeout.Infinite);
            }
        }
    }
}

