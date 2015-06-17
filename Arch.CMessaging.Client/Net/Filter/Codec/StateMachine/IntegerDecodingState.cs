using System;
using Arch.CMessaging.Client.Net.Core.Buffer;

namespace Arch.CMessaging.Client.Net.Filter.Codec.StateMachine
{
    public abstract class IntegerDecodingState : IDecodingState
    {
        private Int32 _firstByte;
        private Int32 _secondByte;
        private Int32 _thirdByte;
        private Int32 _counter;

        public IDecodingState Decode(IoBuffer input, IProtocolDecoderOutput output)
        {
            while (input.HasRemaining)
            {
                switch (_counter)
                {
                    case 0:
                        _firstByte = input.Get() & 0xff;
                        break;
                    case 1:
                        _secondByte = input.Get() & 0xff;
                        break;
                    case 2:
                        _thirdByte = input.Get() & 0xff;
                        break;
                    case 3:
                        _counter = 0;
                        return FinishDecode((_firstByte << 24) | (_secondByte << 16) | (_thirdByte << 8) | (input.Get() & 0xff), output);
                }

                _counter++;
            }
            return this;
        }

        public IDecodingState FinishDecode(IProtocolDecoderOutput output)
        {
            throw new ProtocolDecoderException("Unexpected end of session while waiting for a integer.");
        }
        protected abstract IDecodingState FinishDecode(Int32 value, IProtocolDecoderOutput output);
    }
}
