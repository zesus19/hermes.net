using System;
using Arch.CMessaging.Client.Core.Ioc;

namespace Arch.CMessaging.Client.Consumer.Engine.Config
{
    [Named(ServiceType = typeof(ConsumerConfig))]
    public class ConsumerConfig
    {
        public String DefautlLocalCacheSize
        { 
            get { return "50"; }
        }

        public long RenewLeaseTimeMillisBeforeExpired
        { 
            get { return 2 * 1000L; }
        }

        public long StopConsumerTimeMillsBeforLeaseExpired
        { 
            get { return 500L; }
        }

        public long DefaultLeaseAcquireDelayMillis
        { 
            get { return 500L; }
        }

        public long DefaultLeaseRenewDelayMillis
        { 
            get { return 500L; }
        }

        public String DefaultLocalCachePrefetchThresholdPercentage
        { 
            get { return "30"; }
        }

        public int NoMessageWaitIntervalMillis
        {
            get { return 50; }
        }

        public int NoEndpointWaitIntervalMillis
        {
            get { return 500; }
        }

        public String DefaultNotifierThreadCount
        { 
            get { return "1"; }
        }
    }
}

