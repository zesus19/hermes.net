using System;
using Arch.CMessaging.Client.Net.Core.Session;

namespace Arch.CMessaging.Client.Net.Filter.Codec.Serialization
{
    public class ObjectSerializationCodecFactory : IProtocolCodecFactory
    {
        private readonly ObjectSerializationEncoder _encoder = new ObjectSerializationEncoder();
        private readonly ObjectSerializationDecoder _decoder = new ObjectSerializationDecoder();

        public IProtocolEncoder GetEncoder(IoSession session)
        {
            return _encoder;
        }

        
        public IProtocolDecoder GetDecoder(IoSession session)
        {
            return _decoder;
        }

        public Int32 EncoderMaxObjectSize
        {
            get { return _encoder.MaxObjectSize; }
            set { _encoder.MaxObjectSize = value; }
        }

        public Int32 DecoderMaxObjectSize
        {
            get { return _decoder.MaxObjectSize; }
            set { _decoder.MaxObjectSize = value; }
        }
    }
}
