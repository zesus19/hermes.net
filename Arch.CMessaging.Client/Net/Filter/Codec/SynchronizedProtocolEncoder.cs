using System;
using Arch.CMessaging.Client.Net.Core.Session;

namespace Arch.CMessaging.Client.Net.Filter.Codec
{
    public class SynchronizedProtocolEncoder : IProtocolEncoder
    {
        private readonly IProtocolEncoder _encoder;

        public SynchronizedProtocolEncoder(IProtocolEncoder encoder)
        {
            if (encoder == null)
                throw new ArgumentNullException("encoder");
            _encoder = encoder;
        } 

        public void Encode(IoSession session, Object message, IProtocolEncoderOutput output)
        {
            lock (_encoder)
            {
                _encoder.Encode(session, message, output);
            }
        }

        public void Dispose(IoSession session)
        {
            lock (_encoder)
            {
                _encoder.Dispose(session);
            }
        }
    }
}
