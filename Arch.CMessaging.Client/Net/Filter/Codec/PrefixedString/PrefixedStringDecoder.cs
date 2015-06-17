using System;
using System.Text;
using Arch.CMessaging.Client.Net.Core.Buffer;
using Arch.CMessaging.Client.Net.Core.Session;

namespace Arch.CMessaging.Client.Net.Filter.Codec.PrefixedString
{
    public class PrefixedStringDecoder : CumulativeProtocolDecoder
    {
        public PrefixedStringDecoder(Encoding encoding)
            : this(encoding, PrefixedStringCodecFactory.DefaultPrefixLength, PrefixedStringCodecFactory.DefaultMaxDataLength)
        { }

        public PrefixedStringDecoder(Encoding encoding, Int32 prefixLength)
            : this(encoding, prefixLength, PrefixedStringCodecFactory.DefaultMaxDataLength)
        { }

        public PrefixedStringDecoder(Encoding encoding, Int32 prefixLength, Int32 maxDataLength)
        {
            Encoding = encoding;
            PrefixLength = prefixLength;
            MaxDataLength = maxDataLength;
        }

        public Int32 PrefixLength { get; set; }

        public Int32 MaxDataLength { get; set; }

        public Encoding Encoding { get; set; }

        protected override Boolean DoDecode(IoSession session, IoBuffer input, IProtocolDecoderOutput output)
        {
            if (input.PrefixedDataAvailable(PrefixLength, MaxDataLength))
            {
                String msg = input.GetPrefixedString(PrefixLength, Encoding);
                output.Write(msg);
                return true;
            }

            return false;
        }
    }
}
