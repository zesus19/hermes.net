using System;
using Arch.CMessaging.Client.Net.Core.Buffer;

namespace Arch.CMessaging.Client.Net.Filter.Codec.StateMachine
{
    public abstract class ShortIntegerDecodingState : IDecodingState
    {
        private Int32 _highByte;
        private Int32 _counter;

        public IDecodingState Decode(IoBuffer input, IProtocolDecoderOutput output)
        {
            while (input.HasRemaining)
            {
                switch (_counter)
                {
                    case 0:
                        _highByte = input.Get() & 0xff;
                        break;
                    case 1:
                        _counter = 0;
                        return FinishDecode((Int16)((_highByte << 8) | (input.Get() & 0xff)), output);
                }

                _counter++;
            }
            return this;
        }

        public IDecodingState FinishDecode(IProtocolDecoderOutput output)
        {
            throw new ProtocolDecoderException("Unexpected end of session while waiting for a short integer.");
        }
        protected abstract IDecodingState FinishDecode(Int16 value, IProtocolDecoderOutput output);
    }
}
