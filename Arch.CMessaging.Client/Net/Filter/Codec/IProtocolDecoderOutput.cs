using System;
using Arch.CMessaging.Client.Net.Core.Filterchain;
using Arch.CMessaging.Client.Net.Core.Session;

namespace Arch.CMessaging.Client.Net.Filter.Codec
{
    public interface IProtocolDecoderOutput
    {
        void Write(Object message);
        void Flush(INextFilter nextFilter, IoSession session);
    }
}
