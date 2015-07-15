using System;
using Freeway.Logging;
using System.Collections.Generic;
using Arch.CMessaging.Client.Core.Config;
using Arch.CMessaging.Client.MetaEntity.Entity;
using Arch.CMessaging.Client.Core.Lease;
using Arch.CMessaging.Client.Core.Message.Retry;
using Arch.CMessaging.Client.Core.Bo;
using System.Threading;
using Arch.CMessaging.Client.Core.Ioc;

namespace Arch.CMessaging.Client.Core.MetaService.Internal
{
    [Named(ServiceType = (typeof(IMetaService)))]
    public class DefaultMetaService : IMetaService, IInitializable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DefaultMetaService));

        [Inject]
        private IMetaManager manager;

        [Inject]
        private CoreConfig config;

        private volatile Meta MetaCache = null;

        public Timer timer { get; set; }

        public MetaRefresher metaRefresher { get; set; }

		
        public String FindEndpointTypeByTopic(String topicName)
        {
            Topic topic = MetaCache.FindTopic(topicName);
            if (topic == null)
            {
                throw new Exception(String.Format("Topic {0} not found", topicName));
            }

            return topic.EndpointType;
        }

		
        public List<Partition> ListPartitionsByTopic(String topicName)
        {
            Topic topic = MetaCache.FindTopic(topicName);
            if (topic != null)
            {
                return topic.Partitions;
            }
            else
            {
                throw new Exception(String.Format("Topic {0} not found", topicName));
            }
        }

		
        public Storage FindStorageByTopic(String topicName)
        {
            Topic topic = MetaCache.FindTopic(topicName);
            if (topic == null)
            {
                throw new Exception(String.Format("Topic {0} not found", topicName));
            }
            String storageType = topic.StorageType;
            return MetaCache.FindStorage(storageType);
        }

		
        public Codec FindCodecByTopic(String topicName)
        {
            Topic topic = MetaCache.FindTopic(topicName);
            if (topic != null)
            {
                String codeType = topic.CodecType;
                return MetaCache.Codecs[codeType];
            }
            else
            {
                throw new Exception(String.Format("Topic {0} not found", topicName));
            }
        }

		
        public Partition FindPartitionByTopicAndPartition(String topicName, int partitionId)
        {
            Topic topic = MetaCache.FindTopic(topicName);
            if (topic != null)
            {
                return topic.FindPartition(partitionId);
            }
            else
            {
                throw new Exception(String.Format("Topic {0} not found", topicName));
            }
        }

        public List<Topic> ListTopicsByPattern(String topicPattern)
        {
            if (string.IsNullOrEmpty(topicPattern))
            {
                throw new Exception("Topic pattern can not be null or blank");
            }

            topicPattern = topicPattern.Trim();

            bool hasWildcard = topicPattern.EndsWith("*");

            if (hasWildcard)
            {
                topicPattern = topicPattern.Substring(0, topicPattern.Length - 1);
            }

            List<Topic> matchedTopics = new List<Topic>();
            foreach (Topic topic in MetaCache.Topics.Values)
            {
                if (hasWildcard)
                {
                    if (topic.Name.ToLower().StartsWith(topicPattern.ToLower()))
                    {
                        matchedTopics.Add(topic);
                    }
                }
                else
                {
                    if (topic.Name.ToLower().Equals(topicPattern.ToLower()))
                    {
                        matchedTopics.Add(topic);
                    }
                }
            }

            return matchedTopics;
        }

		
        public Topic FindTopicByName(String topic)
        {
            return MetaCache.FindTopic(topic);
        }

		
        public int TranslateToIntGroupId(String topicName, String groupName)
        {
            Topic topic = FindTopicByName(topicName);

            if (topic == null)
            {
                throw new Exception(String.Format("Topic {0} not found", topicName));
            }

            ConsumerGroup consumerGroup = topic.FindConsumerGroup(groupName);

            if (consumerGroup != null)
            {
                return consumerGroup.ID;
            }
            else
            {
                throw new Exception(String.Format("Consumer group not found for topic {0} and group {1}", topicName,
                        groupName));
            }
        }

        public void Refresh()
        {
            RefreshMeta(manager.LoadMeta());
        }

        private void RefreshMeta(Meta meta)
        {
            MetaCache = meta;
        }

		
        public int GetAckTimeoutSecondsTopicAndConsumerGroup(String topicName, String groupId)
        {
            Topic topic = FindTopicByName(topicName);
            if (topic == null)
            {
                throw new Exception(String.Format("Topic {0} not found", topicName));
            }

            ConsumerGroup consumerGroup = topic.FindConsumerGroup(groupId);

            if (consumerGroup == null)
            {
                throw new Exception(String.Format("Consumer group {0} for topic {1} not found", groupId, topicName));
            }

            if (consumerGroup.AckTimeoutSeconds == 0)
            {
                return topic.AckTimeoutSeconds;
            }
            else
            {
                return consumerGroup.AckTimeoutSeconds;
            }
        }

		
        public LeaseAcquireResponse TryAcquireConsumerLease(Tpg tpg, String sessionId)
        {
            return manager.GetMetaProxy().TryAcquireConsumerLease(tpg, sessionId);
        }

		
        public LeaseAcquireResponse TryRenewConsumerLease(Tpg tpg, ILease lease, String sessionId)
        {
            return manager.GetMetaProxy().TryRenewConsumerLease(tpg, lease, sessionId);
        }

		
        public LeaseAcquireResponse TryRenewBrokerLease(String topic, int partition, ILease lease, String sessionId,
                                                        int brokerPort)
        {
            return manager.GetMetaProxy().TryRenewBrokerLease(topic, partition, lease, sessionId, brokerPort);
        }

		
        public LeaseAcquireResponse TryAcquireBrokerLease(String topic, int partition, String sessionId, int brokerPort)
        {
            return manager.GetMetaProxy().TryAcquireBrokerLease(topic, partition, sessionId, brokerPort);
        }

        public void Initialize()
        {

            RefreshMeta(manager.LoadMeta());


            int interval = (int)config.MetaCacheRefreshIntervalMinutes * 60 * 1000;
            metaRefresher = new MetaRefresher(this, interval);
            timer = new Timer(metaRefresher.Refresh, null, interval, Timeout.Infinite);
        }

		
        public String FindAvroSchemaRegistryUrl()
        {
            Codec avroCodec = MetaCache.FindCodec(Codec.AVRO);
            return avroCodec.Properties[config.AvroSchemaRetryUrlKey].Value;
        }

		
        public Endpoint FindEndpointByTopicAndPartition(String topic, int partition)
        {
            return MetaCache.FindEndpoint(FindTopicByName(topic).FindPartition(partition).Endpoint);
        }

		
        public IRetryPolicy FindRetryPolicyByTopicAndGroup(String topicName, String groupId)
        {
            Topic topic = FindTopicByName(topicName);
            if (topic == null)
            {
                throw new Exception(String.Format("Topic {0} not found", topicName));
            }

            ConsumerGroup consumerGroup = topic.FindConsumerGroup(groupId);

            if (consumerGroup == null)
            {
                throw new Exception(String.Format("Consumer group {0} for topic {1} not found", groupId, topicName));
            }

            String retryPolicyValue = consumerGroup.RetryPolicy;
            if (string.IsNullOrEmpty(retryPolicyValue))
            {
                retryPolicyValue = topic.ConsumerRetryPolicy;
            }

            return RetryPolicyFactory.Create(retryPolicyValue);
        }

		
        public List<SubscriptionView> ListSubscriptions()
        {
            return manager.GetMetaProxy().ListSubscriptions();
        }

		
        public List<SchemaView> ListSchemas()
        {
            return manager.GetMetaProxy().ListSchemas();
        }

		
        public bool ContainsEndpoint(Endpoint endpoint)
        {
            return MetaCache.Endpoints.ContainsKey(endpoint.ID);
        }

        public bool ContainsConsumerGroup(String topicName, String groupId)
        {
            Topic topic = FindTopicByName(topicName);
            if (topic == null)
            {
                throw new Exception(string.Format("Topic {0} not found", topicName));
            }

            ConsumerGroup consumerGroup = topic.FindConsumerGroup(groupId);

            if (consumerGroup == null)
            {
                return false;
            }

            return true;
        }

        public class MetaRefresher
        {

            private DefaultMetaService metaService;
            private int interval;

            public MetaRefresher(DefaultMetaService metaService, int interval)
            {
                this.metaService = metaService;
                this.interval = interval;
            }

            public void Refresh(object param)
            {
                try
                {
                    metaService.timer.Change(Timeout.Infinite, Timeout.Infinite);
                    metaService.Refresh();
                }
                catch (Exception e)
                {
                    log.Warn("Failed to refresh meta", e);
                }
                finally
                {
                    metaService.timer.Change(interval, Timeout.Infinite);
                }
            }
        }
    }
}

