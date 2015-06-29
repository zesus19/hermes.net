using System;
using Arch.CMessaging.Client.Transport.EndPoint;
using Arch.CMessaging.Client.Core.MetaService;
using Arch.CMessaging.Client.Consumer.Engine.Notifier;

namespace Arch.CMessaging.Client.Consumer.Engine.Bootstrap
{
	public abstract class BaseConsumerBootstrap : IConsumerBootstrap
	{
		protected IEndpointClient EndpointClient;

		protected IEndpointManager EndpointManager;

		protected IMetaService MetaService;

		protected IConsumerNotifier ConsumerNotifier;

		public ISubscribeHandle start (ConsumerContext context)
		{
			return doStart (context);
		}	

		public void stop (ConsumerContext context)
		{
			doStop (context);
		}

		protected void doStop (ConsumerContext context)
		{

		}

		protected abstract ISubscribeHandle doStart (ConsumerContext context);
	}
}

