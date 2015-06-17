using Arch.CMessaging.Client.Net.Core.Buffer;
using Arch.CMessaging.Client.Net.Core.Session;

namespace Arch.CMessaging.Client.Net.Filter.Codec
{
    public interface IProtocolDecoder
    {
        void Decode(IoSession session, IoBuffer input, IProtocolDecoderOutput output);
        void FinishDecode(IoSession session, IProtocolDecoderOutput output);
        void Dispose(IoSession session);
    }
}
