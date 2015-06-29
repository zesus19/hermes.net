using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arch.CMessaging.Client.Core.Config
{
    public class CoreConfig
    {
        public int CommandProcessorThreadCount { get { return 10; } }
        public int MetaServerIpFetchInterval { get { return 60; } }
        public int MetaServerConnectTimeoutInMills { get { return 2000; } }
        public int MetaServerReadTimeoutInMills { get { return 5000; } }
        public long RunningStatusStatInterval { get { return 30; } }
        public long MetaCacheRefreshIntervalMinutes { get { return 1; } }
        public int SendBufferSize { get { return 65535; } }
        public int ReceiveBufferSize { get {return 65535; } }
        public int EndpointSessionSendBufferSize { get { return 1000; } }
        public int EndpointSessionWriterCheckInterval { get { return 20; } }
        public int EndpointSessionWriteRetryDealyInMills { get { return 20; } }
        public int EndpointSessionAutoReconnectDelay { get { return 1; } }
        public int EndpointSessionDefaultWrtieTimeoutInMills { get { return 3600 * 1000; } }
        public int EndpointSessionMaxIdleTime { get { return 60; } }
        public string AvroSchemaRetryUrlKey { get { return "schema.registry.url"; } }
    }
}
