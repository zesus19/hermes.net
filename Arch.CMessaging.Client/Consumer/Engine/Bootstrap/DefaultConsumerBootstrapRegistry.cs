using System;
using Arch.CMessaging.Client.Core.Ioc;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Arch.CMessaging.Client.Core.Utils;

namespace Arch.CMessaging.Client.Consumer.Engine.Bootstrap
{
    [Named(ServiceType = typeof(IConsumerBootstrapRegistry))]
    public class DefaultConsumerBootstrapRegistry : IConsumerBootstrapRegistry, IInitializable
    {
        private ConcurrentDictionary<string, IConsumerBootstrap> bootstraps = new ConcurrentDictionary<string, IConsumerBootstrap>();

        public void Initialize()
        {
            IDictionary<string, IConsumerBootstrap> bootstraps = ComponentLocator.LookupMap<IConsumerBootstrap>();

            foreach (KeyValuePair<string, IConsumerBootstrap> entry in bootstraps)
            {
                this.bootstraps[entry.Key] = entry.Value;
            }
        }

        public void RegisterBootstrap(String endpointType, IConsumerBootstrap consumerBootstrap)
        {
            if (bootstraps.ContainsKey(endpointType))
            {
                throw new Exception(string.Format("ConsumerBootstrap for endpoint type {0} is already registered", endpointType));
            }

            bootstraps[endpointType] = consumerBootstrap;
        }

        public IConsumerBootstrap FindConsumerBootstrap(String endpointType)
        {
            IConsumerBootstrap bootstrap;
            bootstraps.TryGetValue(endpointType, out bootstrap);
            return bootstrap;
        }
    }
}

