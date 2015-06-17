using System;
using Arch.CMessaging.Client.Net.Core.Buffer;

namespace Arch.CMessaging.Client.Net.Filter.Codec.StateMachine
{
    public abstract class ConsumeToEndOfSessionDecodingState : IDecodingState
    {
        private readonly Int32 _maxLength;
        private IoBuffer _buffer;

        protected ConsumeToEndOfSessionDecodingState(Int32 maxLength)
        {
            _maxLength = maxLength;
        }

        public IDecodingState Decode(IoBuffer input, IProtocolDecoderOutput output)
        {
            if (_buffer == null)
            {
                _buffer = IoBuffer.Allocate(256);
                _buffer.AutoExpand = true;
            }

            if (_buffer.Position + input.Remaining > _maxLength)
                throw new ProtocolDecoderException("Received data exceeds " + _maxLength + " byte(s).");
         
            _buffer.Put(input);
            return this;
        }

        public IDecodingState FinishDecode(IProtocolDecoderOutput output)
        {
            try
            {
                if (_buffer == null)
                {
                    _buffer = IoBuffer.Allocate(0);
                }
                _buffer.Flip();
                return FinishDecode(_buffer, output);
            }
            finally
            {
                _buffer = null;
            }
        }

        protected abstract IDecodingState FinishDecode(IoBuffer product, IProtocolDecoderOutput output);
    }
}
