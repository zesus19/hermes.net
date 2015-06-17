using System;
using Arch.CMessaging.Client.Net.Core.Buffer;

namespace Arch.CMessaging.Client.Net.Filter.Codec.StateMachine
{
    public abstract class SkippingState : IDecodingState
    {
        private Int32 _skippedBytes;

        public IDecodingState Decode(IoBuffer input, IProtocolDecoderOutput output)
        {
            Int32 beginPos = input.Position;
            Int32 limit = input.Limit;
            for (Int32 i = beginPos; i < limit; i++)
            {
                Byte b = input.Get(i);
                if (!CanSkip(b))
                {
                    input.Position = i;
                    Int32 answer = _skippedBytes;
                    _skippedBytes = 0;
                    return FinishDecode(answer);
                }

                _skippedBytes++;
            }

            input.Position = limit;
            return this;
        }

        public IDecodingState FinishDecode(IProtocolDecoderOutput output)
        {
            return FinishDecode(_skippedBytes);
        }

        protected abstract Boolean CanSkip(Byte b);
        protected abstract IDecodingState FinishDecode(Int32 skippedBytes);
    }
}
