using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Arch.CMessaging.Client.Core.Utils;
using Arch.CMessaging.Client.Core.Ioc;

namespace Arch.CMessaging.Client.Consumer.Engine.Bootstrap.Strategy
{
    public class DefaultBrokerConsumptionRegistry : IBrokerConsumptionStrategyRegistry, IInitializable
    {
        private ConcurrentDictionary<ConsumerType, IBrokerConsumptionStrategy> m_strategies = new ConcurrentDictionary<ConsumerType, IBrokerConsumptionStrategy>();

        public void Initialize()
        {
            IDictionary<string, IBrokerConsumptionStrategy> strategies = ComponentLocator.LookupMap<IBrokerConsumptionStrategy>();

            foreach (KeyValuePair<string, IBrokerConsumptionStrategy> entry in strategies)
            {
                ConsumerType consumerType;
                Enum.TryParse<ConsumerType>(entry.Key, out consumerType);
                m_strategies.TryAdd(consumerType, entry.Value);
            }
        }

        public IBrokerConsumptionStrategy FindStrategy(ConsumerType consumerType)
        {
            return CollectionUtil.TryGet(m_strategies, consumerType);
        }
    }
}

