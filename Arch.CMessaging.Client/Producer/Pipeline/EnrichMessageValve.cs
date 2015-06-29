﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arch.CMessaging.Client.Core.Pipeline.spi;
using Arch.CMessaging.Client.Core.Pipeline;
using Arch.CMessaging.Client.Core.Ioc;
using Arch.CMessaging.Client.Core.Pipeline.spi;
using Freeway.Logging;
using Arch.CMessaging.Client.Core.Pipeline;
using Arch.CMessaging.Client.Core.Message;

namespace Arch.CMessaging.Client.Producer.Pipeline
{
	[Named]
	public class EnrichMessageValve : IValve
	{
		private static readonly ILog log = LogManager.GetLogger (typeof(EnrichMessageValve));

		public static string ID = "enrich";

		public void Handle (IPipelineContext ctx, Object payload)
		{
			ProducerMessage msg = (ProducerMessage)payload;
			String topic = msg.Topic;
			String ip = "";

			if (string.IsNullOrEmpty (topic)) {
				log.Error ("Topic not set, won't send");
				return;
			}

			enrichPartitionKey (msg, ip);
			enrichRefKey (msg);
			enrichMessageProperties (msg, ip);

			ctx.Next (payload);
		}

		private void enrichRefKey (ProducerMessage msg)
		{
			if (string.IsNullOrEmpty (msg.Key)) {
				String refKey = System.Guid.NewGuid ().ToString ();
				log.Info (string.Format ("Ref key not set, will set uuid as ref key(topic={0}, ref key={1})", msg.Topic, refKey));
				msg.Key = refKey;
			}
		}

		private void enrichMessageProperties (ProducerMessage msg, String ip)
		{
			msg.AddDurableSysProperty (MessagePropertyNames.PRODUCER_IP, ip);
		}

		private void enrichPartitionKey (ProducerMessage msg, String ip)
		{
			if (string.IsNullOrEmpty (msg.PartitionKey)) {
				log.Info (string.Format ("Parition key not set, will set ip as partition key(topic={0}, ip={1})", msg.Topic, ip));
				msg.PartitionKey = ip;
			}
		}
	}
}
