using System;
using Arch.CMessaging.Client.MetaEntity.Entity;
using Arch.CMessaging.Client.Core.Ioc;

namespace Arch.CMessaging.Client.Consumer.Engine.Bootstrap
{
    [Named(ServiceType = typeof(IConsumerBootstrapManager))]
    public class DefaultConsumerBootstrapManager : IConsumerBootstrapManager
    {
        [Inject]
        private IConsumerBootstrapRegistry registry;

        public IConsumerBootstrap FindConsumerBootStrap(String endpointType)
        {

            if (Endpoint.BROKER.Equals(endpointType) || Endpoint.KAFKA.Equals(endpointType))
            {
                return registry.FindConsumerBootstrap(endpointType);
            }
            else
            {
                throw new Exception(string.Format("Unknown endpoint type: {0}", endpointType));
            }

        }
    }
}

