using System;
using Arch.CMessaging.Client.Core.Pipeline;
using System.Collections.Generic;
using Arch.CMessaging.Client.Core.Pipeline.spi;
using Arch.CMessaging.Client.Core.Ioc;
using Arch.CMessaging.Client.Consumer.Build;

namespace Arch.CMessaging.Client.Consumer.Engine.Pipeline
{
    public class ConsumerPipeline : IPipeline<object>
    {
        [Inject(BuildConstants.CONSUMER)]
        private IPipelineSink m_pipelineSink;

        [Inject(BuildConstants.CONSUMER)]
        private IValveRegistry m_registry;

        public object Put(object payload)
        {
            IList<IValve> valves = m_registry.GetValveList();
            IPipelineContext ctx = new DefaultPipelineContext(valves, m_pipelineSink);

            ctx.Next(payload);

            return null;
        }
    }
}

