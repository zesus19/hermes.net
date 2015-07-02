using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arch.CMessaging.Client.Core.Ioc;
using Arch.CMessaging.Client.Producer.Monitor;

namespace Arch.CMessaging.Client.Transport.Command.Processor
{
    [Named(ServiceType = typeof(ICommandProcessor), ServiceName = "SendMessageResultCommandProcessor")]
    public class SendMessageResultCommandProcessor : ICommandProcessor
    {
        [Inject]
        private ISendMessageResultMonitor messageResultMonitor;

        #region ICommandProcessor Members

        public List<CommandType> CommandTypes()
        {
            return new List<CommandType> { CommandType.ResultMessageSend };
        }

        public void Process(CommandProcessorContext ctx)
        {
            var command = ctx.Command as SendMessageResultCommand;
            messageResultMonitor.ResultReceived(command);
        }

        #endregion
    }
}
