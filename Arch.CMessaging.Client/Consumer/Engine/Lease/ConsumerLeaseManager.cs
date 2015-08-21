using System;
using Arch.CMessaging.Client.Core.Bo;
using Arch.CMessaging.Client.Core.Lease;
using Arch.CMessaging.Client.Core.MetaService;
using Arch.CMessaging.Client.Consumer.Build;
using Arch.CMessaging.Client.Core.Ioc;

namespace Arch.CMessaging.Client.Consumer.Engine.Lease
{
    [Named(ServiceType = typeof(ILeaseManager<ConsumerLeaseKey>), ServiceName = BuildConstants.CONSUMER)]
    public class ConsumerLeaseManager : ILeaseManager<ConsumerLeaseKey>
    {
        [Inject]
        private IMetaService MetaService;

        public LeaseAcquireResponse TryAcquireLease(ConsumerLeaseKey key)
        {
            return MetaService.TryAcquireConsumerLease(key.Tpg, key.SessionId);
        }

        public LeaseAcquireResponse TryRenewLease(ConsumerLeaseKey key, ILease lease)
        {
            return MetaService.TryRenewConsumerLease(key.Tpg, lease, key.GetSessionId());
        }
    }

    public class ConsumerLeaseKey : ISessionIdAware
    {
        public Tpg Tpg { get; private set; }

        public String SessionId { get; private set; }

        public ConsumerLeaseKey(Tpg tpg, String sessionId)
        {
            this.Tpg = tpg;
            SessionId = sessionId;
        }

        public String GetSessionId()
        {
            return SessionId;
        }

        public override int GetHashCode()
        {
            int prime = 31;
            int result = 1;
            result = prime * result + ((SessionId == null) ? 0 : SessionId.GetHashCode());
            result = prime * result + ((Tpg == null) ? 0 : Tpg.GetHashCode());
            return result;
        }

        public  override bool Equals(Object obj)
        {
            if (this == obj)
                return true;
            if (obj == null)
                return false;
            if (GetType() != obj.GetType())
                return false;
            ConsumerLeaseKey other = (ConsumerLeaseKey)obj;
            if (SessionId == null)
            {
                if (other.SessionId != null)
                    return false;
            }
            else if (!SessionId.Equals(other.SessionId))
                return false;
            if (Tpg == null)
            {
                if (other.Tpg != null)
                    return false;
            }
            else if (!Tpg.Equals(other.Tpg))
                return false;
            return true;
        }

    }
}

