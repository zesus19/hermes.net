using System;
using System.Text;
using Arch.CMessaging.Client.Net.Core.Session;

namespace Arch.CMessaging.Client.Net.Filter.Codec.PrefixedString
{
    public class PrefixedStringCodecFactory : IProtocolCodecFactory
    {
        public const Int32 DefaultPrefixLength = 4;
        public const Int32 DefaultMaxDataLength = 2048;

        private readonly PrefixedStringEncoder _encoder;
        private readonly PrefixedStringDecoder _decoder;

        public PrefixedStringCodecFactory()
            : this(Encoding.Default)
        { }

        public PrefixedStringCodecFactory(Encoding encoding)
        {
            _encoder = new PrefixedStringEncoder(encoding);
            _decoder = new PrefixedStringDecoder(encoding);
        }

        public Int32 EncoderPrefixLength
        {
            get { return _encoder.PrefixLength; }
            set { _encoder.PrefixLength = value; }
        }
        
        public Int32 EncoderMaxDataLength
        {
            get { return _encoder.MaxDataLength; }
            set { _encoder.MaxDataLength = value; }
        }

        public Int32 DecoderPrefixLength
        {
            get { return _decoder.PrefixLength; }
            set { _decoder.PrefixLength = value; }
        }
        
        public Int32 DecoderMaxDataLength
        {
            get { return _decoder.MaxDataLength; }
            set { _decoder.MaxDataLength = value; }
        }

        public IProtocolEncoder GetEncoder(IoSession session)
        {
            return _encoder;
        }

        public IProtocolDecoder GetDecoder(IoSession session)
        {
            return _decoder;
        }
    }
}
