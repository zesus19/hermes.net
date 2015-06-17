using System;
using Arch.CMessaging.Client.Net.Core.Session;

namespace Arch.CMessaging.Client.Net.Filter.Codec
{
    public interface IProtocolEncoder
    {
        void Encode(IoSession session, Object message, IProtocolEncoderOutput output);
        void Dispose(IoSession session);
    }
}
