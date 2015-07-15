using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Arch.CMessaging.Client.Core.Utils;
using Arch.CMessaging.Client.Core.Ioc;

namespace Arch.CMessaging.Client.Consumer.Engine.Bootstrap.Strategy
{
    [Named(ServiceType = typeof(IBrokerConsumptionStrategyRegistry))]
    public class DefaultBrokerConsumptionRegistry : IBrokerConsumptionStrategyRegistry, IInitializable
    {
        private ConcurrentDictionary<ConsumerType, IBrokerConsumptionStrategy> Strategies = new ConcurrentDictionary<ConsumerType, IBrokerConsumptionStrategy>();

        public void Initialize()
        {
            IDictionary<string, IBrokerConsumptionStrategy> strategies = ComponentLocator.LookupMap<IBrokerConsumptionStrategy>();

            foreach (KeyValuePair<string, IBrokerConsumptionStrategy> entry in strategies)
            {
                ConsumerType consumerType;
                Enum.TryParse<ConsumerType>(entry.Key, out consumerType);
                Strategies[consumerType] = entry.Value;
            }
        }

        public IBrokerConsumptionStrategy FindStrategy(ConsumerType consumerType)
        {
            return CollectionUtil.TryGet(Strategies, consumerType);
        }
    }
}

