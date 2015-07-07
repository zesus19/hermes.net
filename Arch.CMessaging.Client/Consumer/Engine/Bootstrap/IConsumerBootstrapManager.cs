using System;

namespace Arch.CMessaging.Client.Consumer.Engine.Bootstrap
{
    public interface IConsumerBootstrapManager
    {
        IConsumerBootstrap findConsumerBootStrap(String endpointType);
    }
}

