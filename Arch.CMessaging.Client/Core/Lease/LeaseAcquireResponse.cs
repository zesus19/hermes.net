using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arch.CMessaging.Client.Newtonsoft.Json;

namespace Arch.CMessaging.Client.Core.Lease
{
    public class LeaseAcquireResponse
    {

        public LeaseAcquireResponse()
        {
        }

        public LeaseAcquireResponse(bool acquired, DefaultLease lease, long nextTryTime)
        {
            this.Acquired = acquired;
            this.Lease = lease;
            this.NextTryTime = nextTryTime;
        }

        [JsonProperty(PropertyName = "acquired")]
        public bool Acquired { get; set; }

        [JsonProperty(PropertyName = "lease")]
        public DefaultLease Lease { get; set; }

        [JsonProperty(PropertyName = "nextTryTime")]
        public long NextTryTime { get; set; }

        public override string ToString()
        {
            return string.Format("[LeaseAcquireResponse: Acquired={0}, Lease={1}, NextTryTime={2}]", Acquired, Lease, NextTryTime);
        }
    }
}
