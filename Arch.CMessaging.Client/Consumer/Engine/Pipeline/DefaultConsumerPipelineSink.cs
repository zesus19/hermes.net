using System;
using Arch.CMessaging.Client.Core.Pipeline;
using Freeway.Logging;
using Arch.CMessaging.Client.Core.Service;
using Arch.CMessaging.Client.Core.Utils;
using System.Collections.Generic;
using Arch.CMessaging.Client.Core.Message;
using Arch.CMessaging.Client.Core.Ioc;
using Arch.CMessaging.Client.Consumer.Build;

namespace Arch.CMessaging.Client.Consumer.Engine.Pipeline
{
    [Named(ServiceType = typeof(IPipelineSink), ServiceName = BuildConstants.CONSUMER)]
    public class DefaultConsumerPipelineSink : IPipelineSink
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DefaultConsumerPipelineSink));

        [Inject]
        private ISystemClockService m_systemClockService;

        public object Handle(IPipelineContext ctx, Object payload)
        {
            Pair<ConsumerContext, List<IConsumerMessage>> pair = (Pair<ConsumerContext, List<IConsumerMessage>>)payload;

            IMessageListener consumer = pair.Key.Consumer;
            List<IConsumerMessage> msgs = pair.Value;
            setOnMessageStartTime(msgs);
            try
            {
                consumer.OnMessage(msgs);
            }
            catch (Exception e)
            {
                log.Error("Uncaught exception occurred while calling MessageListener's onMessage method, will nack all messages which handled by this call.", e);
                foreach (IConsumerMessage msg in msgs)
                {
                    msg.nack();
                }
            }
            finally
            {
                foreach (IConsumerMessage msg in msgs)
                {
                    // ensure every message is acked or nacked, ack it if not
                    msg.ack();
                }
            }

            return null;
        }

        private void setOnMessageStartTime(List<IConsumerMessage> msgs)
        {
            foreach (IConsumerMessage msg in msgs)
            {
                if (msg is BaseConsumerMessageAware)
                {
                    BaseConsumerMessage baseMsg = ((BaseConsumerMessageAware)msg).BaseConsumerMessage;
                    baseMsg.OnMessageStartTimeMills = m_systemClockService.Now();
                }
            }
        }
    }
}

