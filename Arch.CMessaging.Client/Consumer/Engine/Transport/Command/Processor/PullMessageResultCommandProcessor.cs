using System;
using Arch.CMessaging.Client.Transport.Command.Processor;
using System.Collections.Generic;
using Arch.CMessaging.Client.Transport.Command;
using Arch.CMessaging.Client.Consumer.Engine.Monitor;
using Arch.CMessaging.Client.Core.Ioc;

namespace Arch.CMessaging.Client.Consumer.Engine.Transport.Command.Processor
{
    public class PullMessageResultCommandProcessor : ICommandProcessor
    {
        [Inject]
        private IPullMessageResultMonitor m_messageResultMonitor;

        public List<CommandType> CommandTypes()
        {
            return new List<CommandType>{ CommandType.ResultMessagePull };
        }

        public void Process(CommandProcessorContext ctx)
        {
            PullMessageResultCommand cmd = (PullMessageResultCommand)ctx.Command;
            cmd.Channel = ctx.Session;
            m_messageResultMonitor.resultReceived(cmd);
        }
    }
}

