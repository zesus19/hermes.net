using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arch.CMessaging.Client.Core.Future;
using Arch.CMessaging.Client.Core.Message;
using Arch.CMessaging.Client.Core.Pipeline;
using Arch.CMessaging.Client.Core.Result;


namespace Arch.CMessaging.Client.Producer.Pipeline
{
    public class ProducerPipeline : IPipeline<IFuture<SendResult>>
    {
        //ioc inject
        private IValveRegistry valveRegistry;
        //ioc inject 
        private IProducerPipelineSinkManager sinkManager;

        public IFuture<SendResult> Put(object payload)
        {
            var message = payload as ProducerMessage;
            var sink = sinkManager.GetSink(message.Topic);
            var context = new DefaultPipelineContext<IFuture<SendResult>>(valveRegistry.GetValveList(), sink);
            context.Next(message);
            return context.GetResult();
        }
    }
}
