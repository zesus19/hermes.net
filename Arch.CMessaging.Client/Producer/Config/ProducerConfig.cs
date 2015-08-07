using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arch.CMessaging.Client.Core.Ioc;

namespace Arch.CMessaging.Client.Producer.Config
{
	[Named (ServiceType = typeof(ProducerConfig
	))]
	public class ProducerConfig
	{
		public string DefaultBrokerSenderNetworkIoThreadCount { get { return "10"; } }

        public string DefaultBrokerSenderNetworkIoCheckIntervalBaseMillis { get { return "5"; } }

		public string DefaultBrokerSenderNetworkIoCheckIntervalMaxMillis { get { return "50"; } }

		public string DefaultBrokerSenderBatchSize { get { return "10000"; } }

		public int DefaultBrokerSenderSendTimeoutMillis { get { return 10000; } }

		public string DefaultBrokerSenderTaskQueueSize { get { return "500000"; } }

		public string DefaultProducerCallbackThreadCount { get { return "3"; } }

		public int SendMessageReadResultTimeoutMillis { get { return 10 * 1000; } }
	}
}
