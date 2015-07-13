using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arch.CMessaging.Client.Core.Utils;
using Arch.CMessaging.Client.Core.MetaService;
using Arch.CMessaging.Client.MetaEntity.Entity;

namespace Arch.CMessaging.Client.Core.Message.Payload
{
    public class PayloadCodecFactory
    {
        static JsonPayloadCodec jsonCodec = new JsonPayloadCodec();

        public static IPayloadCodec GetCodecByTopicName(string topic)
        {
            IMetaService metaService = ComponentLocator.Lookup<IMetaService>();

            Arch.CMessaging.Client.MetaEntity.Entity.Codec codecEntity = metaService.FindCodecByTopic(topic);
            return GetCodecByType(codecEntity.Type);
        }

        public static IPayloadCodec GetCodecByType(string codecType)
        {
            return ComponentLocator.Lookup<IPayloadCodec>(codecType);
        }
    }
}
