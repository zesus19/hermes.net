using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arch.CMessaging.Client.Net.Core.Buffer;
using Arch.CMessaging.Client.Core.Utils;

namespace Arch.CMessaging.Client.Core.Message
{
    public class PartialDecodedMessage
    {
        public string BodyCodecType { get; set; }

        public IoBuffer Body{ get; set; }

        public string Key{ get; set; }

        public long BornTime{ get; set; }

        public int RemainingRetries { get; set; }

        public IoBuffer DurableProperties{ get; set; }

        public IoBuffer VolatileProperties{ get; set; }

        public bool WithHeader{ get; set; }

        public byte[] readBody()
        {
            return readByteBuf(Body);
        }

        public byte[] readDurableProperties()
        {
            return readByteBuf(DurableProperties);
        }

        public byte[] readVolatileProperties()
        {
            return readByteBuf(VolatileProperties);
        }

        private byte[] readByteBuf(IoBuffer buf)
        {
            if (buf == null)
            {
                return null;
            }

            return buf.GetRemainingArray();
        }
    }
}
