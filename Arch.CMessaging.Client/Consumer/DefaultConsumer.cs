using System;
using Arch.CMessaging.Client.Consumer.Engine;
using Arch.CMessaging.Client.Core.Ioc;

namespace Arch.CMessaging.Client.Consumer
{
    public class DefaultConsumer : Consumer
    {
        [Inject]
        private IEngine m_engine;

        private IConsumerHolder Start(String topic, String groupId, IMessageListener listener, ConsumerType consumerType)
        {
            ISubscribeHandle subscribeHandle = m_engine.Start(new System.Collections.Generic.List<Subscriber>{ new Subscriber(topic, groupId, listener, consumerType) });

            return new DefaultConsumerHolder(subscribeHandle);
        }

        public  override IConsumerHolder Start(String topic, String groupId, IMessageListener listener)
        {
            return Start(topic, groupId, listener, ConsumerType.LONG_POLLING);
        }

        public class DefaultConsumerHolder : IConsumerHolder
        {

            private ISubscribeHandle m_subscribeHandle;

            public DefaultConsumerHolder(ISubscribeHandle subscribeHandle)
            {
                m_subscribeHandle = subscribeHandle;
            }

            public void close()
            {
                m_subscribeHandle.Close();
            }

        }
    }
}

