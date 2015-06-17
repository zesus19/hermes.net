using System;
using Arch.CMessaging.Client.Net.Core.Buffer;
using Arch.CMessaging.Client.Net.Core.Session;

namespace Arch.CMessaging.Client.Net.Filter.Codec
{
    public abstract class ProtocolDecoderAdapter : IProtocolDecoder
    {
        public abstract void Decode(IoSession session, IoBuffer input, IProtocolDecoderOutput output);

        public virtual void FinishDecode(IoSession session, IProtocolDecoderOutput output)
        {
            // Do nothing
        }

        public virtual void Dispose(IoSession session)
        {
            // Do nothing
        }
    }
}
