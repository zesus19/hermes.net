using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arch.CMessaging.Client.Core.Message.Payload
{
    public class PayloadCodecFactory
    {
        static JsonPayloadCodec jsonCodec = new JsonPayloadCodec();
        public static IPayloadCodec GetCodecByTopicName(string topic)
        {
            return jsonCodec;
        }

        public static IPayloadCodec GetCodecByType(string codecType)
        {
            return null;
        }
    }
}
