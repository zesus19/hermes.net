using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arch.CMessaging.Client.Producer.Config
{
    public class ProducerConfig
    {
        public string DefaultBrokerSenderNetworkIoThreadCount { get { return "10"; } }

        public string DefaultBrokerSenderNetworkIoCheckIntervalMillis { get { return "50"; } }

        public string DefaultBrokerSenderBatchSize { get { return "300"; } }

        public string DefaultBrokerSenderSendTimeoutMillis { get { return "200"; } }

        public string DefaultBrokerSenderTaskQueueSize { get { return "10000";} }

        public string DefaultProducerCallbackThreadCount { get { return "3"; } }

        public int SendMessageReadResultTimeoutMillis { get { return 10 * 1000; } }
    }
}
