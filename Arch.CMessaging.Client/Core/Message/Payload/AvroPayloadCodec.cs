using System;
using Avro.Specific;

namespace Arch.CMessaging.Client.Core.Message.Payload
{
	public class AvroPayloadCodec : IPayloadCodec
	{
		public string Type { get { return Arch.CMessaging.Client.MetaEntity.Entity.Codec.AVRO; } }

		public byte[] Encode(string topic, object obj)
		{
			return null;
		}

		public T Decode<T>(byte[] raw)
		{
			throw new NotImplementedException();
		}
	}
}

