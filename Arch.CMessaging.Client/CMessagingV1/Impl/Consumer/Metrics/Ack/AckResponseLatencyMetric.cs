﻿using Arch.CFramework.AppInternals.Components.MetricComponents.Attributes;
using Arch.CMessaging.Core.CFXMetrics;

namespace cmessaging.consumer.ack.response
{
    [MetricScheduling(Circle = 1)]
    public class AckResponseLatencyMetric:LatencyMetricBase
    {
        public AckResponseLatencyMetric()
        {
            Tags.Add("ServerHostName", "");
        }

        [Tag]
        public string ServerHostName
        {
            get { return Tags["ServerHostName"]; }
            set { Tags["ServerHostName"] = value; }
        }
    }
}
