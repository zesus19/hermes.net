using System;

namespace Arch.CMessaging.Client.Consumer.Engine.Bootstrap
{
	public interface IConsumerBootstrap
	{
		ISubscribeHandle Start (ConsumerContext consumerContext);

		void Stop (ConsumerContext consumerContext);
	}
}

