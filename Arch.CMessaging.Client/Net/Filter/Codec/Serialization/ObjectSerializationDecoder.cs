using System;
using Arch.CMessaging.Client.Net.Core.Buffer;
using Arch.CMessaging.Client.Net.Core.Session;

namespace Arch.CMessaging.Client.Net.Filter.Codec.Serialization
{
    public class ObjectSerializationDecoder : CumulativeProtocolDecoder
    {
        private Int32 _maxObjectSize = 1048576; // 1MB

        public Int32 MaxObjectSize
        {
            get { return _maxObjectSize; }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("MaxObjectSize should be larger than zero.", "value");
                _maxObjectSize = value;
            }
        }

        protected override Boolean DoDecode(IoSession session, IoBuffer input, IProtocolDecoderOutput output)
        {
            if (!input.PrefixedDataAvailable(4, _maxObjectSize))
                return false;

            input.GetInt32();
            output.Write(input.GetObject());
            return true;
        }
    }
}
