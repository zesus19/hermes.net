using System;

namespace Arch.CMessaging.Client.Core.Lease
{
	public interface ILeaseManager<T> where T : ISessionIdAware
	{
		LeaseAcquireResponse tryAcquireLease (T key);

		LeaseAcquireResponse tryRenewLease (T key, ILease lease);
	}
}