using System;
using Arch.CMessaging.Client.Core.Lease;
using System.Collections.Generic;
using Arch.CMessaging.Client.Core.Bo;

namespace Arch.CMessaging.Client.Core.MetaService.Internal
{
    public interface IMetaProxy
    {
        LeaseAcquireResponse TryAcquireConsumerLease(Tpg tpg, String sessionId);

        LeaseAcquireResponse TryRenewConsumerLease(Tpg tpg, ILease lease, String sessionId);

        LeaseAcquireResponse TryRenewBrokerLease(String topic, int partition, ILease lease, String sessionId, int brokerPort);

        LeaseAcquireResponse TryAcquireBrokerLease(String topic, int partition, String sessionId, int brokerPort);

        List<SchemaView> ListSchemas();

        List<SubscriptionView> ListSubscriptions();

        int RegisterSchema(String schema, String subject);

        String GetSchemaString(int schemaId);
    }
}

