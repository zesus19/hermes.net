using System;
using Arch.CMessaging.Client.Core.Ioc;

namespace Arch.CMessaging.Client.Consumer.Engine.Config
{
    [Named(ServiceType = typeof(ConsumerConfig))]
    public class ConsumerConfig
    {
        public String GetDefautlLocalCacheSize()
        {
            return "50";
        }

        public long GetRenewLeaseTimeMillisBeforeExpired()
        {
            return 2 * 1000L;
        }

        public long GetStopConsumerTimeMillsBeforLeaseExpired()
        {
            return 500L;
        }

        public long GetDefaultLeaseAcquireDelayMillis()
        {
            return 500L;
        }

        public long GetDefaultLeaseRenewDelayMillis()
        {
            return 500L;
        }

        public String GetDefaultLocalCachePrefetchThresholdPercentage()
        {
            return "30";
        }

        public long GetNoMessageWaitIntervalMillis()
        {
            return 50L;
        }

        public long GetNoEndpointWaitIntervalMillis()
        {
            return 500L;
        }

        public String GetDefaultNotifierThreadCount()
        {
            return "1";
        }
    }
}

