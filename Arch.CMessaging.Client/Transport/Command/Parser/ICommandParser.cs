using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arch.CMessaging.Client.Net.Core.Buffer;

namespace Arch.CMessaging.Client.Transport.Command.Parser
{
    public interface ICommandParser
    {
        ICommand Parse(IoBuffer buf);
    }
}
