using System;
using Arch.CMessaging.Client.Net.Core.Buffer;

namespace Arch.CMessaging.Client.Net.Filter.Codec.StateMachine
{
    public abstract class FixedLengthDecodingState : IDecodingState
    {
        private readonly Int32 _length;
        private IoBuffer _buffer;

        protected FixedLengthDecodingState(Int32 length)
        {
            _length = length;
        }

        public IDecodingState Decode(IoBuffer input, IProtocolDecoderOutput output)
        {
            if (_buffer == null)
            {
                if (input.Remaining >= _length)
                {
                    Int32 limit = input.Limit;
                    input.Limit = input.Position + _length;
                    IoBuffer product = input.Slice();
                    input.Position = input.Position + _length;
                    input.Limit = limit;
                    return FinishDecode(product, output);
                }

                _buffer = IoBuffer.Allocate(_length);
                _buffer.Put(input);
                return this;
            }

            if (input.Remaining >= _length - _buffer.Position)
            {
                Int32 limit = input.Limit;
                input.Limit = input.Position + _length - _buffer.Position;
                _buffer.Put(input);
                input.Limit = limit;
                IoBuffer product = _buffer;
                _buffer = null;
                return FinishDecode(product.Flip(), output);
            }

            _buffer.Put(input);
            return this;
        }

        public IDecodingState FinishDecode(IProtocolDecoderOutput output)
        {
            IoBuffer readData;
            if (_buffer == null)
            {
                readData = IoBuffer.Allocate(0);
            }
            else
            {
                readData = _buffer.Flip();
                _buffer = null;
            }
            return FinishDecode(readData, output);
        }

        protected abstract IDecodingState FinishDecode(IoBuffer product, IProtocolDecoderOutput output);
    }
}
