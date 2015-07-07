using System;

namespace Arch.CMessaging.Client.Consumer.Engine.Bootstrap
{
    public interface IConsumerBootstrapRegistry
    {
        void RegisterBootstrap(String endpointType, IConsumerBootstrap consumerBootstrap);

        IConsumerBootstrap FindConsumerBootstrap(String endpointType);
    }
}

