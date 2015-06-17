using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arch.CMessaging.Client.Core.Future;
using Arch.CMessaging.Client.Core.Message;
using Arch.CMessaging.Client.Core.Pipeline;
using Arch.CMessaging.Client.Core.Result;
using Arch.CMessaging.Client.Producer.Sender;

namespace Arch.CMessaging.Client.Producer.Pipeline
{
    public class DefaultProducerPipelineSink : IPipelineSink<IFuture<SendResult>>
    {
        //ioc inject
        private IMessageSender messageSender;

        public IFuture<SendResult> Handle(IPipelineContext context, object input)
        {
            return messageSender.Send((ProducerMessage)input);
        }
    }
}
