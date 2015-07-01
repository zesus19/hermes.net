using System;
using Arch.CMessaging.Client.Core.Lease;
using System.Collections.Generic;
using Arch.CMessaging.Client.Core.Bo;

namespace Arch.CMessaging.Client.Core.MetaService.Internal
{
	public interface IMetaProxy
	{
		LeaseAcquireResponse tryAcquireConsumerLease (Tpg tpg, String sessionId);

		LeaseAcquireResponse tryRenewConsumerLease (Tpg tpg, ILease lease, String sessionId);

		LeaseAcquireResponse tryRenewBrokerLease (String topic, int partition, ILease lease, String sessionId, int brokerPort);

		LeaseAcquireResponse tryAcquireBrokerLease (String topic, int partition, String sessionId, int brokerPort);

		List<SchemaView> listSchemas ();

		List<SubscriptionView> listSubscriptions ();

		int registerSchema (String schema, String subject);

		String getSchemaString (int schemaId);
	}
}

