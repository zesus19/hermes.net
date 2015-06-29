using System;
using Freeway.Logging;
using System.Collections.Generic;
using Arch.CMessaging.Client.Core.Config;
using Arch.CMessaging.Client.MetaEntity.Entity;
using Arch.CMessaging.Client.Core.Lease;
using Arch.CMessaging.Client.Core.Message.Retry;
using Arch.CMessaging.Client.Core.Bo;
using System.Threading;

namespace Arch.CMessaging.Client.Core.MetaService.Internal
{
	public class DefaultMetaService : IMetaService
	{
		private static readonly ILog log = LogManager.GetLogger (typeof(DefaultMetaService));

		//@Inject
		private IMetaManager m_manager;

		//@Inject
		private CoreConfig m_config;

		//private ScheduledExecutorService executor;

		private volatile Meta MetaCache = null;

		private Timer timer;

		
		public String FindEndpointTypeByTopic (String topicName)
		{
			Topic topic = MetaCache.FindTopic (topicName);
			if (topic == null) {
				throw new Exception (String.Format ("Topic {0} not found", topicName));
			}

			return topic.EndpointType;
		}

		
		public List<Partition> ListPartitionsByTopic (String topicName)
		{
			Topic topic = MetaCache.FindTopic (topicName);
			if (topic != null) {
				return topic.Partitions;
			} else {
				throw new Exception (String.Format ("Topic {0} not found", topicName));
			}
		}

		
		public Storage FindStorageByTopic (String topicName)
		{
			Topic topic = MetaCache.FindTopic (topicName);
			if (topic == null) {
				throw new Exception (String.Format ("Topic {0} not found", topicName));
			}
			String storageType = topic.StorageType;
			return MetaCache.FindStorage (storageType);
		}

		
		public Codec FindCodecByTopic (String topicName)
		{
			Topic topic = MetaCache.FindTopic (topicName);
			if (topic != null) {
				String codeType = topic.CodecType;
				return MetaCache.Codecs [codeType];
			} else {
				throw new Exception (String.Format ("Topic {0} not found", topicName));
			}
		}

		
		public Partition FindPartitionByTopicAndPartition (String topicName, int partitionId)
		{
			Topic topic = MetaCache.FindTopic (topicName);
			if (topic != null) {
				return topic.FindPartition (partitionId);
			} else {
				throw new Exception (String.Format ("Topic {0} not found", topicName));
			}
		}

		public List<Topic> ListTopicsByPattern (String topicPattern)
		{
			if (string.IsNullOrEmpty (topicPattern)) {
				throw new Exception ("Topic pattern can not be null or blank");
			}

			topicPattern = topicPattern.Trim ();

			bool hasWildcard = topicPattern.EndsWith ("*");

			if (hasWildcard) {
				topicPattern = topicPattern.Substring (0, topicPattern.Length - 1);
			}

			List<Topic> matchedTopics = new List<Topic> ();
			foreach (Topic topic in MetaCache.Topics.Values) {
				if (hasWildcard) {
					if (topic.Name.ToLower ().StartsWith (topicPattern.ToLower ())) {
						matchedTopics.Add (topic);
					}
				} else {
					if (topic.Name.ToLower ().Equals (topicPattern.ToLower ())) {
						matchedTopics.Add (topic);
					}
				}
			}

			return matchedTopics;
		}

		
		public Topic FindTopicByName (String topic)
		{
			return MetaCache.FindTopic (topic);
		}

		
		public int TranslateToIntGroupId (String topicName, String groupName)
		{
			Topic topic = FindTopicByName (topicName);

			if (topic == null) {
				throw new Exception (String.Format ("Topic {0} not found", topicName));
			}

			ConsumerGroup consumerGroup = topic.FindConsumerGroup (groupName);

			if (consumerGroup != null) {
				return consumerGroup.ID;
			} else {
				throw new Exception (String.Format ("Consumer group not found for topic {0} and group {1}", topicName,
					groupName));
			}
		}

		public void Refresh ()
		{
			RefreshMeta (m_manager.loadMeta ());
		}

		private void RefreshMeta (Meta meta)
		{
			MetaCache = meta;
		}

		
		public int GetAckTimeoutSecondsTopicAndConsumerGroup (String topicName, String groupId)
		{
			Topic topic = FindTopicByName (topicName);
			if (topic == null) {
				throw new Exception (String.Format ("Topic {0} not found", topicName));
			}

			ConsumerGroup consumerGroup = topic.FindConsumerGroup (groupId);

			if (consumerGroup == null) {
				throw new Exception (String.Format ("Consumer group {0} for topic {1} not found", groupId, topicName));
			}

			if (consumerGroup.AckTimeoutSeconds == null) {
				return topic.AckTimeoutSeconds;
			} else {
				return consumerGroup.AckTimeoutSeconds;
			}
		}

		
		public LeaseAcquireResponse TryAcquireConsumerLease (Tpg tpg, String sessionId)
		{
			return m_manager.getMetaProxy ().tryAcquireConsumerLease (tpg, sessionId);
		}

		
		public LeaseAcquireResponse TryRenewConsumerLease (Tpg tpg, ILease lease, String sessionId)
		{
			return m_manager.getMetaProxy ().tryRenewConsumerLease (tpg, lease, sessionId);
		}

		
		public LeaseAcquireResponse TryRenewBrokerLease (String topic, int partition, ILease lease, String sessionId,
		                                                 int brokerPort)
		{
			return m_manager.getMetaProxy ().tryRenewBrokerLease (topic, partition, lease, sessionId, brokerPort);
		}

		
		public LeaseAcquireResponse TryAcquireBrokerLease (String topic, int partition, String sessionId, int brokerPort)
		{
			return m_manager.getMetaProxy ().tryAcquireBrokerLease (topic, partition, sessionId, brokerPort);
		}

		
		public void Initialize ()
		{
			/*
			RefreshMeta(m_manager.loadMeta());

			timer = new Timer ();

			executor = Executors.newSingleThreadScheduledExecutor(HermesThreadFactory.create("RefreshMeta", true));
			executor
				.scheduleWithFixedDelay(new Runnable() {

					
					public void run() {
						try {
							refresh();
						} catch (Exception e) {
							log.warn("Failed to refresh meta", e);
						}
					}

				}, m_config.getMetaCacheRefreshIntervalMinutes(), m_config.getMetaCacheRefreshIntervalMinutes(),
					TimeUnit.MINUTES);
					*/
		}

		
		public String FindAvroSchemaRegistryUrl ()
		{
			Codec avroCodec = MetaCache.FindCodec (Codec.AVRO);
			return avroCodec.Properties [m_config.AvroSchemaRetryUrlKey].Value;
		}

		
		public Endpoint FindEndpointByTopicAndPartition (String topic, int partition)
		{
			return MetaCache.FindEndpoint (FindTopicByName (topic).FindPartition (partition).Endpoint);
		}

		
		public IRetryPolicy FindRetryPolicyByTopicAndGroup (String topicName, String groupId)
		{
			Topic topic = FindTopicByName (topicName);
			if (topic == null) {
				throw new Exception (String.Format ("Topic {0} not found", topicName));
			}

			ConsumerGroup consumerGroup = topic.FindConsumerGroup (groupId);

			if (consumerGroup == null) {
				throw new Exception (String.Format ("Consumer group {0} for topic {1} not found", groupId, topicName));
			}

			String retryPolicyValue = consumerGroup.RetryPolicy;
			if (string.IsNullOrEmpty (retryPolicyValue)) {
				retryPolicyValue = topic.ConsumerRetryPolicy;
			}

			return RetryPolicyFactory.create (retryPolicyValue);
		}

		
		public List<SubscriptionView> ListSubscriptions ()
		{
			return null;
			/*
			return m_manager.getMetaProxy().listSubscriptions();
			*/
		}

		
		public List<SchemaView> ListSchemas ()
		{
			return null;
			/*
			return m_manager.getMetaProxy().listSchemas();
			*/
		}

		
		public bool ContainsEndpoint (Endpoint endpoint)
		{
			return true;
			/*
			return m_metaCache.get().getEndpoints().containsKey(endpoint.getId());
			*/
		}
	}
}

