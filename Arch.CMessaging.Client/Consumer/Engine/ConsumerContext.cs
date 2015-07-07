using System;
using Arch.CMessaging.Client.MetaEntity.Entity;

namespace Arch.CMessaging.Client.Consumer.Engine
{
    public class ConsumerContext
    {
        public Topic Topic { get; private set; }

        public String GroupId { get; private set; }

        public Type MessageClazz { get; private set; }

        public IMessageListener Consumer { get; private set; }

        public ConsumerType ConsumerType { get; private set; }

        public String SessionId { get; private set; }

        public ConsumerContext(Topic topic, String groupId, IMessageListener consumer, Type messageClazz,
                               ConsumerType consumerType)
        {
            Topic = topic;
            GroupId = groupId;
            Consumer = consumer;
            MessageClazz = messageClazz;
            ConsumerType = consumerType;
            SessionId = Guid.NewGuid().ToString();
        }
    }
}

