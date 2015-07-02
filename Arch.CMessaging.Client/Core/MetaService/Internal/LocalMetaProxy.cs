using System;
using Arch.CMessaging.Client.Core.Ioc;
using Arch.CMessaging.Client.Core.Utils;
using Arch.CMessaging.Client.Core.Lease;
using Arch.CMessaging.Client.Core.Bo;
using System.Collections.Generic;
using Arch.CMessaging.Client.Core.MetaService.Internal;

namespace Arch.CMessaging.Client.Core.MetaService.Internal
{
	[Named (ServiceType = typeof(IMetaProxy), ServiceName = LocalMetaProxy.ID)]
	public class LocalMetaProxy : IMetaProxy
	{
		public const String ID = "local";

		private ThreadSafe.Long m_leaseId = new ThreadSafe.Long (0);

		public LeaseAcquireResponse tryAcquireConsumerLease (Tpg tpg, String sessionId)
		{
			long expireTime = new DateTime ().CurrentTimeMillis () + 10 * 1000L;
			long leaseId = m_leaseId.AtomicIncrementAndGet ();
			return new LeaseAcquireResponse (true, new DefaultLease (leaseId, expireTime), expireTime);
		}

		
		public LeaseAcquireResponse tryRenewConsumerLease (Tpg tpg, ILease lease, String sessionId)
		{
			return new LeaseAcquireResponse (false, null, new DateTime ().CurrentTimeMillis () + 10 * 1000L);
		}

		
		public LeaseAcquireResponse tryRenewBrokerLease (String topic, int partition, ILease lease, String sessionId,
		                                                 int brokerPort)
		{
			long expireTime = new DateTime ().CurrentTimeMillis () + 10 * 1000L;
			long leaseId = m_leaseId.AtomicIncrementAndGet ();
			return new LeaseAcquireResponse (true, new DefaultLease (leaseId, expireTime), expireTime);
		}

		
		public LeaseAcquireResponse tryAcquireBrokerLease (String topic, int partition, String sessionId, int brokerPort)
		{
			long expireTime = new DateTime ().CurrentTimeMillis () + 10 * 1000L;
			long leaseId = m_leaseId.AtomicIncrementAndGet ();
			return new LeaseAcquireResponse (true, new DefaultLease (leaseId, expireTime), expireTime);
		}

		
		public List<SchemaView> listSchemas ()
		{
			return new List<SchemaView> ();
		}

		
		public List<SubscriptionView> listSubscriptions ()
		{
			return new List<SubscriptionView> ();
		}

		public int registerSchema (String schema, String subject)
		{
			throw new NotImplementedException ();
		}

		public String getSchemaString (int schemaId)
		{
			throw new NotImplementedException ();
		}
	}
}

