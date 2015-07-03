using System;
using Arch.CMessaging.Client.Transport.EndPoint;
using Arch.CMessaging.Client.Core.MetaService;
using Arch.CMessaging.Client.Consumer.Engine.Notifier;
using Arch.CMessaging.Client.Core.Ioc;

namespace Arch.CMessaging.Client.Consumer.Engine.Bootstrap
{
    [Named(ServiceType = typeof(IConsumerBootstrap))]
    public abstract class BaseConsumerBootstrap : IConsumerBootstrap
    {
        [Inject]
        protected IEndpointClient EndpointClient;

        [Inject]
        protected IEndpointManager EndpointManager;

        [Inject]
        protected IMetaService MetaService;

        [Inject]
        protected IConsumerNotifier ConsumerNotifier;

        public ISubscribeHandle start(ConsumerContext context)
        {
            return doStart(context);
        }

        public void stop(ConsumerContext context)
        {
            doStop(context);
        }

        protected void doStop(ConsumerContext context)
        {

        }

        protected abstract ISubscribeHandle doStart(ConsumerContext context);
    }
}

