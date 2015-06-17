using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arch.CMessaging.Client.Transport.Command;
using Arch.CMessaging.Client.Net.Filter.Codec.Demux;
using Arch.CMessaging.Client.Net.Core.Session;
using Arch.CMessaging.Client.Net.Filter.Codec;
using Arch.CMessaging.Client.Net.Core.Buffer;

namespace Arch.CMessaging.Client.Transport
{
    public class CommandEncoder : IMessageEncoder<ICommand>
    {
        #region IMessageEncoder<TCommand> Members

        public void Encode(IoSession session, ICommand message, IProtocolEncoderOutput output)
        {
            var buf = IoBuffer.Allocate(1024);
            buf.AutoExpand = true;
            message.ToBytes(buf);
            buf.Flip();
            output.Write(buf);
        }

        #endregion

        #region IMessageEncoder Members

        public void Encode(IoSession session, object message, IProtocolEncoderOutput output)
        {
            Encode(session, (ICommand)message, output);
        }

        #endregion

           
    }
}
