using Arch.CMessaging.Client.Net.Core.Session;

namespace Arch.CMessaging.Client.Net.Filter.Codec
{
    public interface IProtocolCodecFactory
    {
        IProtocolEncoder GetEncoder(IoSession session);
        IProtocolDecoder GetDecoder(IoSession session);
    }
}
