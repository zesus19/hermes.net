using System;
using Avro.Specific;
using Arch.CMessaging.Client.Core.Ioc;

namespace Arch.CMessaging.Client.Core.Message.Payload
{
    [Named(ServiceType = typeof(IPayloadCodec), ServiceName = Arch.CMessaging.Client.MetaEntity.Entity.Codec.AVRO)]
    public class AvroPayloadCodec : IPayloadCodec
    {
        public string Type { get { return Arch.CMessaging.Client.MetaEntity.Entity.Codec.AVRO; } }

        public byte[] Encode(string topic, object obj)
        {
            return null;
        }

        public object Decode(byte[] raw, Type type)
        {
            throw new NotImplementedException();
        }
    }
}

