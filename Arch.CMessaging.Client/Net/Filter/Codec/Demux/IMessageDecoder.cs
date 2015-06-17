using Arch.CMessaging.Client.Net.Core.Buffer;
using Arch.CMessaging.Client.Net.Core.Session;

namespace Arch.CMessaging.Client.Net.Filter.Codec.Demux
{
    public interface IMessageDecoder
    {
        MessageDecoderResult Decodable(IoSession session, IoBuffer input);
        MessageDecoderResult Decode(IoSession session, IoBuffer input, IProtocolDecoderOutput output);
        void FinishDecode(IoSession session, IProtocolDecoderOutput output);
    }
}
