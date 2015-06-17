using System;
using Arch.CMessaging.Client.Net.Core.Buffer;

namespace Arch.CMessaging.Client.Net.Filter.Codec.StateMachine
{
    public abstract class SingleByteDecodingState : IDecodingState
    {
        public IDecodingState Decode(IoBuffer input, IProtocolDecoderOutput output)
        {
            if (input.HasRemaining)
                return FinishDecode(input.Get(), output);

            return this;
        }

        public IDecodingState FinishDecode(IProtocolDecoderOutput output)
        {
            throw new ProtocolDecoderException("Unexpected end of session while waiting for a single byte.");
        }
        protected abstract IDecodingState FinishDecode(Byte b, IProtocolDecoderOutput output);
    }
}
