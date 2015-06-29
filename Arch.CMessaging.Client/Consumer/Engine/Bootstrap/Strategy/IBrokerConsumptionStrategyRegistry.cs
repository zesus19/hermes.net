using System;

namespace Arch.CMessaging.Client.Consumer.Engine.Bootstrap.Strategy
{
	public interface IBrokerConsumptionStrategyRegistry
	{
		IBrokerConsumptionStrategy FindStrategy (ConsumerType consumerType);
	}
}

