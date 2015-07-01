using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arch.CMessaging.Client.Core.Env;
using Arch.CMessaging.Client.Core.Message.Partition;
using Arch.CMessaging.Client.MetaEntity.Entity;
using Arch.CMessaging.Client.Core.Pipeline;
using Arch.CMessaging.Client.Core.Service;
using Arch.CMessaging.Client.Core.Utils;
using Arch.CMessaging.Client.Producer.Config;
using Arch.CMessaging.Client.Producer.Monitor;
using Arch.CMessaging.Client.Producer.Pipeline;
using Arch.CMessaging.Client.Producer.Sender;
using Arch.CMessaging.Client.Transport.Command;
using Arch.CMessaging.Client.Transport.Command.Processor;
using Arch.CMessaging.Client.Transport.EndPoint;
using Arch.CMessaging.Client.Core.MetaService.Internal;
using Arch.CMessaging.Client.Core.Message.Codec;
using Arch.CMessaging.Client.Core.Future;
using Arch.CMessaging.Client.Core.Result;
using Arch.CMessaging.Client.Core.Message;

namespace Arch.CMessaging.Client.Producer.Build
{
    public class ComponentsConfigurator
    {
        public static void DefineComponents()
        {
            ComponentLocator.DefineComponents(c =>
                {
                    //c.Define<DefaultProducer>();
                    //c.Define<ProducerPipeline>();
                    //c.Define<IValveRegistry, ProducerValveRegistry>(BuildConstants.PRODUCER);
                    //c.Define<IMessageCodec, DefaultMessageCodec>();

                    ////valves
                    //c.Define<TracingMessageValve>();
                    //c.Define<EnrichMessageValve>();

                    ////sinks
                    //c.Define<DefaultProducerPipelineSinkManager>();
                    //c.Define<IPipelineSink, DefaultProducerPipelineSink>(Endpoint.BROKER);
                    //c.Define<IMessageSender, BrokerMessageSender>(Endpoint.BROKER);
                    //c.Define<DefaultEndpointManager>();
                    //c.Define<HashPartitioningStrategy>();
                    //c.Define<DefaultMetaService>();
                    //c.Define<DefaultSendMessageAcceptanceMonitor>();
                    //c.Define<DefaultSendMessageResultMonitor>();
                    //c.Define<ProducerConfig>();
                    //c.Define<DefaultEndpointClient>();
                    //c.Define<DefaultClientEnvironment>();
                    //c.Define<DefaultSystemClockService>();
                    //c.Define<ICommandProcessor, SendMessageAckCommandProcessor>(CommandType.AckMessageSend.ToString());
                    //c.Define<ICommandProcessor, SendMessageResultCommandProcessor>(CommandType.ResultMessageSend.ToString());
                });
        }
    }
}
