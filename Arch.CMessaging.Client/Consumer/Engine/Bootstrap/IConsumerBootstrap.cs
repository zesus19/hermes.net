using System;

namespace Arch.CMessaging.Client.Consumer.Engine.Bootstrap
{
	public interface IConsumerBootstrap
	{
		ISubscribeHandle start (ConsumerContext consumerContext);

		void stop (ConsumerContext consumerContext);
	}
}

