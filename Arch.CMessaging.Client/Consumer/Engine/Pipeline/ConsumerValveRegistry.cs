using System;
using Arch.CMessaging.Client.Core.Pipeline;
using Arch.CMessaging.Client.Consumer.Build;
using Arch.CMessaging.Client.Core.Ioc;

namespace Arch.CMessaging.Client.Consumer.Engine.Pipeline
{
    [Named(ServiceType = typeof(IValveRegistry), ServiceName = BuildConstants.CONSUMER)]
    public class ConsumerValveRegistry : AbstractValveRegistry
    {
    }
}

