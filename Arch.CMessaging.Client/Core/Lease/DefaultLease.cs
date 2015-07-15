using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arch.CMessaging.Client.Core.Service;
using Arch.CMessaging.Client.Core.Utils;
using Arch.CMessaging.Client.Newtonsoft.Json;

namespace Arch.CMessaging.Client.Core.Lease
{
    public class DefaultLease : ILease
    {
        private ThreadSafe.Long expireTime;

        public DefaultLease()
            : this(0, 0)
        {
        }

        public DefaultLease(long id, long expireTime)
        {
            this.ID = id;
            this.expireTime = new ThreadSafe.Long(expireTime);
        }

        [JsonProperty(PropertyName = "id")]
        public long ID { get; set; }

        [JsonProperty(PropertyName = "expireTime")]
        public long ExpireTime
        {
            get { return expireTime.ReadFullFence(); }
            set { expireTime.WriteFullFence(value); }
        }

        #region ILease Members

        [JsonProperty(PropertyName = "expired")]
        public bool Expired
        {
            get { return RemainingTime <= 0; }
        }

        [JsonProperty(PropertyName = "remainingTime")]
        public long RemainingTime
        {
            get
            {
                var systemClockService = ComponentLocator.Lookup<ISystemClockService>();
                return ExpireTime - systemClockService.Now();
            }
        }

        #endregion

        public bool Equals(DefaultLease lease)
        {
            return this.ID == lease.ID;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return obj is DefaultLease && Equals((DefaultLease)obj);
        }

        public override int GetHashCode()
        {
            return this.ID.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("[DefaultLease: ID={0}, ExpireTime={1}, Expired={2}, RemainingTime={3}]", ID, ExpireTime, Expired, RemainingTime);
        }
    }
}
