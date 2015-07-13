using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arch.CMessaging.Client.MetaEntity.Entity;
using Arch.CMessaging.Client.Newtonsoft.Json;
using Arch.CMessaging.Client.Core.Utils;
using Arch.CMessaging.Client.Core.Ioc;

namespace Arch.CMessaging.Client.Core.Message.Payload
{
    [Named(ServiceType = typeof(IPayloadCodec), ServiceName = Arch.CMessaging.Client.MetaEntity.Entity.Codec.JSON)]
    public class JsonPayloadCodec : IPayloadCodec
    {
        #region IPayloadCodec Members

        public string Type { get { return Arch.CMessaging.Client.MetaEntity.Entity.Codec.JSON; } }

        public byte[] Encode(string topic, object obj)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
        }

        public object Decode(byte[] raw, Type type)
        {
            return JSON.DeserializeObject(raw, type);
        }

        #endregion
    }
}
