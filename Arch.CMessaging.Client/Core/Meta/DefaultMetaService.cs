using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arch.CMessaging.Client.Core.Meta
{
    public class DefaultMetaService : IMetaService
    {
        #region IMetaService Members

        public string FindAvroSchemaRegistryUrl()
        {
            throw new NotImplementedException();
        }

        public Client.Meta.Entity.Codec FindCodecByTopic(string topic)
        {
            throw new NotImplementedException();
        }

        public Client.Meta.Entity.Endpoint FindEndpointByTopicAndPartition(string topic, int partition)
        {
            throw new NotImplementedException();
        }

        public string FindEndpointTypeByTopic(string topic)
        {
            throw new NotImplementedException();
        }

        public Client.Meta.Entity.Partition FindPartitionByTopicAndPartition(string topic, int partition)
        {
            throw new NotImplementedException();
        }

        public Message.Retry.IRetryPolicy FindRetryPolicyByTopicAndGroup(string topic, string groupId)
        {
            throw new NotImplementedException();
        }

        public Client.Meta.Entity.Storage FindStorageByTopic(string topic)
        {
            throw new NotImplementedException();
        }

        public Client.Meta.Entity.Topic FindTopicByName(string topic)
        {
            throw new NotImplementedException();
        }

        public int GetAckTimeoutSecondsTopicAndConsumerGroup(string topic, string groupId)
        {
            throw new NotImplementedException();
        }

        public List<Client.Meta.Entity.DataSource> ListAllMysqlDataSources()
        {
            throw new NotImplementedException();
        }

        public List<Client.Meta.Entity.Partition> ListPartitionsByTopic(string topic)
        {
            throw new NotImplementedException();
        }

        public List<Client.Meta.Entity.Topic> ListTopicsByPattern(string topicPattern)
        {
            throw new NotImplementedException();
        }

        public Lease.LeaseAcquireResponse TryRenewBrokerLease(string topic, int partition, Lease.ILease lease, string sessionId, int brokerPort)
        {
            throw new NotImplementedException();
        }

        public int TranslateToIntGroupId(string topic, string groupId)
        {
            throw new NotImplementedException();
        }

        public Lease.LeaseAcquireResponse TryAcquireBrokerLease(string topic, int partition, string sessionId, int brokerPort)
        {
            throw new NotImplementedException();
        }

        public Lease.LeaseAcquireResponse TryAcquireConsumerLease(Bo.Tpg tpg, string sessionId)
        {
            throw new NotImplementedException();
        }

        public Lease.LeaseAcquireResponse TryRenewConsumerLease(Bo.Tpg tpg, Lease.ILease lease, string sessionId)
        {
            throw new NotImplementedException();
        }

        public List<Bo.SubscriptionView> ListSubscriptions()
        {
            throw new NotImplementedException();
        }

        public List<Bo.SchemaView> ListSchemas()
        {
            throw new NotImplementedException();
        }

        public void Refresh()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
