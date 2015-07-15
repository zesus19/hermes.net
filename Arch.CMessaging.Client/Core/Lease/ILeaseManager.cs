using System;

namespace Arch.CMessaging.Client.Core.Lease
{
    public interface ILeaseManager<T> where T : ISessionIdAware
    {
        LeaseAcquireResponse TryAcquireLease(T key);

        LeaseAcquireResponse TryRenewLease(T key, ILease lease);
    }
}