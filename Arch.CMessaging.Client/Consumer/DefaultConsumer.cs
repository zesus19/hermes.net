using System;
using Arch.CMessaging.Client.Consumer.Engine;
using Arch.CMessaging.Client.Core.Ioc;

namespace Arch.CMessaging.Client.Consumer
{
    [Named(ServiceType = typeof(Consumer))]
    public class DefaultConsumer : Consumer
    {
        [Inject]
        private IEngine engine;

        private IConsumerHolder Start(String topic, String groupId, IMessageListener listener, ConsumerType consumerType)
        {
            ISubscribeHandle subscribeHandle = engine.Start(new System.Collections.Generic.List<Subscriber>{ new Subscriber(topic, groupId, listener, consumerType) });

            return new DefaultConsumerHolder(subscribeHandle);
        }

        public  override IConsumerHolder Start(String topic, String groupId, IMessageListener listener)
        {
            return Start(topic, groupId, listener, ConsumerType.LONG_POLLING);
        }

        public class DefaultConsumerHolder : IConsumerHolder
        {

            private ISubscribeHandle subscribeHandle;

            public DefaultConsumerHolder(ISubscribeHandle subscribeHandle)
            {
                this.subscribeHandle = subscribeHandle;
            }

            public void Close()
            {
                subscribeHandle.Close();
            }

        }
    }
}

