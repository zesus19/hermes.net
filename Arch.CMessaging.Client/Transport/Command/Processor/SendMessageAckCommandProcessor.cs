using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arch.CMessaging.Client.Core.Ioc;
using Arch.CMessaging.Client.Producer.Monitor;

namespace Arch.CMessaging.Client.Transport.Command.Processor
{
    public class SendMessageAckCommandProcessor : ICommandProcessor
    {
        [Inject]
        private ISendMessageAcceptanceMonitor messageAcceptanceMonitor;


        #region ICommandProcessor Members

        public List<CommandType> CommandTypes()
        {
            return new List<CommandType> { CommandType.AckMessageSend };
        }

        public void Process(CommandProcessorContext ctx)
        {
            var command = ctx.Command as SendMessageAckCommand;
            messageAcceptanceMonitor.Recieved(command.Header.CorrelationId, command.Success);
        }

        #endregion
    }
}
