using System;
using Arch.CMessaging.Client.Consumer.Engine;

namespace Arch.CMessaging.Client.Consumer.Engine.Bootstrap.Strategy
{
	public interface IBrokerConsumptionStrategy
	{
		ISubscribeHandle Start(ConsumerContext context, int partitionId);
	}
}

