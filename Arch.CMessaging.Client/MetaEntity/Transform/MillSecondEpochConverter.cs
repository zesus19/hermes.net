using System;
using Arch.CMessaging.Client.Newtonsoft.Json.Converters;
using Arch.CMessaging.Client.Newtonsoft.Json;

namespace Arch.CMessaging.Client.MetaEntity.Transform
{
	public class MillSecondEpochConverter : DateTimeConverterBase
	{
		private static readonly DateTime _epoch = new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public override void WriteJson (JsonWriter writer, object value, JsonSerializer serializer)
		{
			writer.WriteRawValue (((DateTime)value - _epoch).TotalMilliseconds + "");
		}

		public override object ReadJson (JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.Value == null) {
				return null;
			}
			return _epoch.AddMilliseconds ((long)reader.Value);
		}
	}
}

