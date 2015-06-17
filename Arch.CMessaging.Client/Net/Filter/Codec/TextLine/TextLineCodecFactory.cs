using System;
using System.Text;
using Arch.CMessaging.Client.Net.Core.Session;

namespace Arch.CMessaging.Client.Net.Filter.Codec.TextLine
{
    public class TextLineCodecFactory : IProtocolCodecFactory
    {
        private readonly TextLineEncoder _encoder;
        private readonly TextLineDecoder _decoder;

        public TextLineCodecFactory()
            : this(Encoding.Default)
        { }

        public TextLineCodecFactory(Encoding encoding)
            : this(encoding, LineDelimiter.Unix, LineDelimiter.Auto)
        { }

        public TextLineCodecFactory(Encoding encoding, String encodingDelimiter, String decodingDelimiter)
        {
            _encoder = new TextLineEncoder(encoding, encodingDelimiter);
            _decoder = new TextLineDecoder(encoding, decodingDelimiter);
        }

        public TextLineCodecFactory(Encoding encoding, LineDelimiter encodingDelimiter, LineDelimiter decodingDelimiter)
        {
            _encoder = new TextLineEncoder(encoding, encodingDelimiter);
            _decoder = new TextLineDecoder(encoding, decodingDelimiter);
        }

        
        public IProtocolEncoder GetEncoder(IoSession session)
        {
            return _encoder;
        }

        
        public IProtocolDecoder GetDecoder(IoSession session)
        {
            return _decoder;
        }

        public Int32 EncoderMaxLineLength
        {
            get { return _encoder.MaxLineLength; }
            set { _encoder.MaxLineLength = value; }
        }

        public Int32 DecoderMaxLineLength
        {
            get { return _decoder.MaxLineLength; }
            set { _decoder.MaxLineLength = value; }
        }
    }
}
