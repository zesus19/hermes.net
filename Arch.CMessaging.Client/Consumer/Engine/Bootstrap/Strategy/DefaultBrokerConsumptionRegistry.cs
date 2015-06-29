using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Arch.CMessaging.Client.Consumer.Engine.Bootstrap.Strategy
{
	public class DefaultBrokerConsumptionRegistry : IBrokerConsumptionStrategyRegistry
	{
		/*private ConcurrentDictionary<ConsumerType, IBrokerConsumptionStrategy> m_strategies = new ConcurrentDictionary<ConsumerType, IBrokerConsumptionStrategy>();

		public override void initialize() {
			ConcurrentDictionary<String, IBrokerConsumptionStrategy> strategies = ComponentLocator.LookupMap(BrokerConsumptionStrategy.GetType);

			foreach (KeyValuePair<String, IBrokerConsumptionStrategy> entry in strategies) {
				m_strategies.put(ConsumerType.valueOf(entry.Key), entry.Value);
			}
		}*/
			
		public IBrokerConsumptionStrategy FindStrategy(ConsumerType consumerType) {
			//return m_strategies.get(consumerType);
			return null;
		}
	}
}

