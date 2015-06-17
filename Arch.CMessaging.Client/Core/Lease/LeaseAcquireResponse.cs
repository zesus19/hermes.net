using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arch.CMessaging.Client.Core.Lease
{
    public class LeaseAcquireResponse
    {

        public LeaseAcquireResponse() { }

        public LeaseAcquireResponse(bool acquired, DefaultLease lease, long nextTryTime)
        {
            this.IsAcquired = acquired;
            this.Lease = lease;
            this.NextTryTime = nextTryTime;
        }

        public bool IsAcquired { get; set; }
        public ILease Lease { get; set; }
        public long NextTryTime { get; set; }
    }
}
