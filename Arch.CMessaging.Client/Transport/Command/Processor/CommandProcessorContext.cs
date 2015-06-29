using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arch.CMessaging.Client.Net.Core.Session;

namespace Arch.CMessaging.Client.Transport.Command.Processor
{
    public class CommandProcessorContext
    {
        public CommandProcessorContext(ICommand command, IoSession session)
        {
            this.Command = command;
            this.Session = session;
        }

        public void Write(ICommand command)
        {
            Session.Write(command);
        }

        public ICommand Command { get; private set; }
        public IoSession Session { get; private set; }
    }
}
