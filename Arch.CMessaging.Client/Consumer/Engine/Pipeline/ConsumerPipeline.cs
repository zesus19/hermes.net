using System;
using Arch.CMessaging.Client.Core.Pipeline;
using System.Collections.Generic;
using Arch.CMessaging.Client.Core.Pipeline.spi;
using Arch.CMessaging.Client.Core.Ioc;
using Arch.CMessaging.Client.Consumer.Build;

namespace Arch.CMessaging.Client.Consumer.Engine.Pipeline
{
    [Named(ServiceType = typeof(IPipeline<object>), ServiceName = BuildConstants.CONSUMER)]
    public class ConsumerPipeline : IPipeline<object>
    {
        [Inject(BuildConstants.CONSUMER)]
        private IPipelineSink pipelineSink;

        [Inject(BuildConstants.CONSUMER)]
        private IValveRegistry registry;

        public object Put(object payload)
        {
            IList<IValve> valves = registry.GetValveList();
            IPipelineContext ctx = new DefaultPipelineContext(valves, pipelineSink);

            ctx.Next(payload);

            return null;
        }
    }
}

