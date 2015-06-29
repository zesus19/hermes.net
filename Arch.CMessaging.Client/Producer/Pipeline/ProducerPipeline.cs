using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arch.CMessaging.Client.Core.Future;
using Arch.CMessaging.Client.Core.Ioc;
using Arch.CMessaging.Client.Core.Message;
using Arch.CMessaging.Client.Core.Pipeline;
using Arch.CMessaging.Client.Core.Result;
using Arch.CMessaging.Client.Producer.Build;


namespace Arch.CMessaging.Client.Producer.Pipeline
{
    [Named]
    public class ProducerPipeline : IPipeline<IFuture<SendResult>>
    {
        [Inject(BuildConstants.PRODUCER)]
        private IValveRegistry valveRegistry;
        
        [Inject]
        private IProducerPipelineSinkManager sinkManager;

        public IFuture<SendResult> Put(object payload)
        {
            var message = payload as ProducerMessage;
            var sink = sinkManager.GetSink(message.Topic);
            var context = new DefaultPipelineContext(valveRegistry.GetValveList(), sink);
            context.Next(message);
            return context.GetResult<IFuture<SendResult>>();
        }
    }
}
