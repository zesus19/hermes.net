using System;
using Arch.CMessaging.Client.MetaEntity.Entity;
using Arch.CMessaging.Client.Core.Ioc;

namespace Arch.CMessaging.Client.Consumer.Engine.Bootstrap
{
    public class DefaultConsumerBootstrapManager : IConsumerBootstrapManager
    {
        [Inject]
        private IConsumerBootstrapRegistry m_registry;

        public IConsumerBootstrap findConsumerBootStrap(String endpointType)
        {

            if (Endpoint.BROKER.Equals(endpointType) || Endpoint.KAFKA.Equals(endpointType))
            {
                return m_registry.FindConsumerBootstrap(endpointType);
            }
            else
            {
                throw new Exception(string.Format("Unknown endpoint type: {0}", endpointType));
            }

        }
    }
}

