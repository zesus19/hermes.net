using System;
using Arch.CMessaging.Client.Net.Core.Buffer;

namespace Arch.CMessaging.Client.Net.Filter.Codec.StateMachine
{
    public abstract class ConsumeToTerminatorDecodingState : IDecodingState
    {
        private readonly Byte _terminator;
        private IoBuffer _buffer;

        protected ConsumeToTerminatorDecodingState(Byte terminator)
        {
            _terminator = terminator;
        }

        public IDecodingState Decode(IoBuffer input, IProtocolDecoderOutput output)
        {
            Int32 terminatorPos = input.IndexOf(_terminator);

            if (terminatorPos >= 0)
            {
                Int32 limit = input.Limit;
                IoBuffer product;

                if (input.Position < terminatorPos)
                {
                    input.Limit = terminatorPos;

                    if (_buffer == null)
                    {
                        product = input.Slice();
                    }
                    else
                    {
                        _buffer.Put(input);
                        product = _buffer.Flip();
                        _buffer = null;
                    }

                    input.Limit = limit;
                }
                else
                {
                    // When input contained only terminator rather than actual data...
                    if (_buffer == null)
                    {
                        product = IoBuffer.Allocate(0);
                    }
                    else
                    {
                        product = _buffer.Flip();
                        _buffer = null;
                    }
                }
                input.Position = terminatorPos + 1;
                return FinishDecode(product, output);
            }

            if (_buffer == null)
            {
                _buffer = IoBuffer.Allocate(input.Remaining);
                _buffer.AutoExpand = true;
            }

            _buffer.Put(input);
            return this;
        }

        public IDecodingState FinishDecode(IProtocolDecoderOutput output)
        {
            IoBuffer product;
            // When input contained only terminator rather than actual data...
            if (_buffer == null)
            {
                product = IoBuffer.Allocate(0);
            }
            else
            {
                product = _buffer.Flip();
                _buffer = null;
            }
            return FinishDecode(product, output);
        }

        protected abstract IDecodingState FinishDecode(IoBuffer product, IProtocolDecoderOutput output);
    }
}
