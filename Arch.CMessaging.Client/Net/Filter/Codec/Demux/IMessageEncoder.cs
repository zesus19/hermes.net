using System;
using Arch.CMessaging.Client.Net.Core.Session;

namespace Arch.CMessaging.Client.Net.Filter.Codec.Demux
{
    public interface IMessageEncoder
    {
        void Encode(IoSession session, Object message, IProtocolEncoderOutput output);
    }

    public interface IMessageEncoder<T> : IMessageEncoder
    {
        void Encode(IoSession session, T message, IProtocolEncoderOutput output);
    }
}
