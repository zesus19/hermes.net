using System;
using Arch.CMessaging.Client.Net.Core.Buffer;
using Arch.CMessaging.Client.Net.Core.Session;

namespace Arch.CMessaging.Client.Net.Filter.Codec
{
    public class SynchronizedProtocolDecoder : IProtocolDecoder
    {
        private readonly IProtocolDecoder _decoder;

        public SynchronizedProtocolDecoder(IProtocolDecoder decoder)
        {
            if (decoder == null)
                throw new ArgumentNullException("decoder");
            _decoder = decoder;
        } 

        public void Decode(IoSession session, IoBuffer input, IProtocolDecoderOutput output)
        {
            lock (_decoder)
            {
                _decoder.Decode(session, input, output);
            }
        }

        public void FinishDecode(IoSession session, IProtocolDecoderOutput output)
        {
            lock (_decoder)
            {
                _decoder.FinishDecode(session, output);
            }
        }

        public void Dispose(IoSession session)
        {
            lock (_decoder)
            {
                _decoder.Dispose(session);
            }
        }
    }
}
