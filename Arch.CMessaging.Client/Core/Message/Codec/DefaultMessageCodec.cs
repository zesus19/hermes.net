using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arch.CMessaging.Client.Core.Ioc;
using Arch.CMessaging.Client.Transport;
using Arch.CMessaging.Client.Net.Core.Buffer;

namespace Arch.CMessaging.Client.Core.Message.Codec
{
	[Named (ServiceType = typeof(IMessageCodec))]
	public class DefaultMessageCodec : IMessageCodec
	{
		private static readonly MessageCodecVersion CURRENT_VERSION = MessageCodecVersion.BINARY_V1;

		#region IMessageCodec Members

		public void Encode (ProducerMessage message, IoBuffer buf)
		{
			Magic.WriteMagic (buf);
			buf.Put (CURRENT_VERSION.Version);
			CURRENT_VERSION.Handler.Encode (message, buf);
		}

		public byte[] Encode (ProducerMessage message)
		{
			return CURRENT_VERSION.Handler.Encode (message, CURRENT_VERSION.Version);
		}

		public void Encode (PartialDecodedMessage msg, IoBuffer buf)
		{
			throw new NotImplementedException ();
		}

		public PartialDecodedMessage DecodePartial (IoBuffer buf)
		{
			throw new NotImplementedException ();
		}

		public BaseConsumerMessage Decode (string topic, IoBuffer buf, Type bodyType)
		{
			throw new NotImplementedException ();
		}

		#endregion

		private MessageCodecVersion GetVersion (IoBuffer buf)
		{
			var versionByte = buf.Get ();
			var version = MessageCodecVersion.ValueOf (versionByte);
			if (version == null)
				throw new ArgumentException (string.Format ("Unknown version {0}", versionByte));
			return version;
		}
	}
}
