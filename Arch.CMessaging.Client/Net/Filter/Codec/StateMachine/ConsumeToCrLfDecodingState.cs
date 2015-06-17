using System;
using Arch.CMessaging.Client.Net.Core.Buffer;

namespace Arch.CMessaging.Client.Net.Filter.Codec.StateMachine
{
    public abstract class ConsumeToCrLfDecodingState : IDecodingState
    {
        private static readonly Byte CR = 13;
        private static readonly Byte LF = 10;
        private Boolean _lastIsCR;
        private IoBuffer _buffer;

        public IDecodingState Decode(IoBuffer input, IProtocolDecoderOutput output)
        {
            Int32 beginPos = input.Position;
            Int32 limit = input.Limit;
            Int32 terminatorPos = -1;

            for (Int32 i = beginPos; i < limit; i++)
            {
                Byte b = input.Get(i);
                if (b == CR)
                {
                    _lastIsCR = true;
                }
                else
                {
                    if (b == LF && _lastIsCR)
                    {
                        terminatorPos = i;
                        break;
                    }
                    _lastIsCR = false;
                }
            }

            if (terminatorPos >= 0)
            {
                IoBuffer product;

                Int32 endPos = terminatorPos - 1;

                if (beginPos < endPos)
                {
                    input.Limit = endPos;

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
                    // When input contained only CR or LF rather than actual data...
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

            input.Position = beginPos;

            if (_buffer == null)
            {
                _buffer = IoBuffer.Allocate(input.Remaining);
                _buffer.AutoExpand = true;
            }

            _buffer.Put(input);

            if (_lastIsCR)
            {
                _buffer.Position = _buffer.Position - 1;
            }

            return this;
        }

        public IDecodingState FinishDecode(IProtocolDecoderOutput output)
        {
            IoBuffer product;
            // When input contained only CR or LF rather than actual data...
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
