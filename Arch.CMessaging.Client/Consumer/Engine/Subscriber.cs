using System;
using Arch.CMessaging.Client.Consumer;

namespace Arch.CMessaging.Client.Consumer.Engine
{
    public class Subscriber
    {
        public string GroupId { get; private set; }

        public string TopicPattern { get; private set; }

        public IMessageListener Consumer{ get; private set; }

        public ConsumerType ConsumerType{ get; private set; }

        public Subscriber(string topicPattern, String groupId, IMessageListener consumer, ConsumerType consumerType)
        {
            TopicPattern = topicPattern;
            GroupId = groupId;
            Consumer = consumer;
            ConsumerType = consumerType;
        }

        public Subscriber(String topicPattern, String groupId, IMessageListener consumer)
            : this(topicPattern, groupId, consumer, ConsumerType.LONG_POLLING)
        {
        }

    }
}

