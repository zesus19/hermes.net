using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arch.CMessaging.Client.Transport.Command
{
    public enum CommandType
    {
        MessageSend = 101,
        MessageAck = 102,
        MessagePull = 130,
        AckMessageSend = 201,
        ResultMessageSend = 301,
        ResultMessagePull = 302
    }
}
