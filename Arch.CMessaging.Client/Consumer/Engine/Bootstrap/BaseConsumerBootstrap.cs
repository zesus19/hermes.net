using System;
using Arch.CMessaging.Client.Transport.EndPoint;
using Arch.CMessaging.Client.Core.MetaService;
using Arch.CMessaging.Client.Consumer.Engine.Notifier;
using Arch.CMessaging.Client.Core.Ioc;

namespace Arch.CMessaging.Client.Consumer.Engine.Bootstrap
{
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

        public ISubscribeHandle Start(ConsumerContext context)
        {
            return DoStart(context);
        }

        public void Stop(ConsumerContext context)
        {
            DoStop(context);
        }

        protected void DoStop(ConsumerContext context)
        {

        }

        protected abstract ISubscribeHandle DoStart(ConsumerContext context);
    }
}

