using System;
using Arch.CMessaging.Client.Net.Core.Session;

namespace Arch.CMessaging.Client.Net.Filter.Codec
{
    public abstract class ProtocolEncoderAdapter : IProtocolEncoder
    {
        public abstract void Encode(IoSession session, Object message, IProtocolEncoderOutput output);

        public virtual void Dispose(IoSession session)
        {
            // Do nothing
        }
    }
}
