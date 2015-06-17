using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arch.CMessaging.Client.Net.Core.Buffer;

namespace Arch.CMessaging.Client.Core.Message.Codec
{
    public interface IMessageCodecHandler
    {
        void Encode(ProducerMessage message, IoBuffer buf);
	    PartialDecodedMessage DecodePartial(IoBuffer buf);
	    BaseConsumerMessage Decode(string topic, IoBuffer buf, Type bodyType);
	    void Encode(PartialDecodedMessage msg, IoBuffer buf);
	    byte[] Encode(ProducerMessage message, byte version);
    }
}
