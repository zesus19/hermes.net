using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arch.CMessaging.Client.Core.Service;
using Arch.CMessaging.Client.Core.Utils;

namespace Arch.CMessaging.Client.Core.Lease
{
    public class DefaultLease : ILease
    {
        private ThreadSafe.Long expireTime;
        public DefaultLease() { }
        public DefaultLease(long id, long expireTime)
        {
            this.ID = id;
            this.expireTime = new ThreadSafe.Long(expireTime);
        }

        public long ID { get; set; }
        public long ExpireTime 
        {
            get { return expireTime.ReadFullFence(); }
            set { expireTime.WriteFullFence(value); }
        }

        #region ILease Members
        public bool IsExpired
        {
            get { return GetRemainingTime() <= 0; }
        }

        public long GetRemainingTime()
        {
            var systemClockService = ComponentLocator.Lookup<ISystemClockService>();
            return ExpireTime - systemClockService.Now();
        }

        #endregion

        public bool Equals(DefaultLease lease)
        {
            return this.ID == lease.ID;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is DefaultLease && Equals((DefaultLease)obj);
        }

        public override int GetHashCode()
        {
            return this.ID.GetHashCode();
        }
    }
}
