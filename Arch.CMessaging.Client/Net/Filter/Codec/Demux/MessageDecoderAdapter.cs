using Arch.CMessaging.Client.Net.Core.Buffer;
using Arch.CMessaging.Client.Net.Core.Session;

namespace Arch.CMessaging.Client.Net.Filter.Codec.Demux
{
    public abstract class MessageDecoderAdapter : IMessageDecoder
    {
        public abstract MessageDecoderResult Decodable(IoSession session, IoBuffer input);

        public abstract MessageDecoderResult Decode(IoSession session, IoBuffer input, IProtocolDecoderOutput output);

        public virtual void FinishDecode(IoSession session, IProtocolDecoderOutput output)
        {
            // Do nothing
        }
    }
}
