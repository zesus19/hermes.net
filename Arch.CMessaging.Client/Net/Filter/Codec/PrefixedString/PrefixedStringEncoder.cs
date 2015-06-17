using System;
using System.Text;
using Arch.CMessaging.Client.Net.Core.Buffer;
using Arch.CMessaging.Client.Net.Core.Session;

namespace Arch.CMessaging.Client.Net.Filter.Codec.PrefixedString
{
    public class PrefixedStringEncoder : ProtocolEncoderAdapter
    {
         public PrefixedStringEncoder(Encoding encoding)
            : this(encoding, PrefixedStringCodecFactory.DefaultPrefixLength, PrefixedStringCodecFactory.DefaultMaxDataLength)
        { }

        public PrefixedStringEncoder(Encoding encoding, Int32 prefixLength)
             : this(encoding, prefixLength, PrefixedStringCodecFactory.DefaultMaxDataLength)
        { }

        public PrefixedStringEncoder(Encoding encoding, Int32 prefixLength, Int32 maxDataLength)
        {
            Encoding = encoding;
            PrefixLength = prefixLength;
            MaxDataLength = maxDataLength;
        }
        public Int32 PrefixLength { get; set; }
        public Int32 MaxDataLength { get; set; }
        public Encoding Encoding { get; set; }
        public override void Encode(IoSession session, Object message, IProtocolEncoderOutput output)
        {
            String value = (String)message;
            IoBuffer buffer = IoBuffer.Allocate(value.Length);
            buffer.AutoExpand = true;
            buffer.PutPrefixedString(value, PrefixLength, Encoding);
            if (buffer.Position > MaxDataLength)
            {
                throw new ArgumentException("Data length: " + buffer.Position);
            }
            buffer.Flip();
            output.Write(buffer);
        }
    }
}
