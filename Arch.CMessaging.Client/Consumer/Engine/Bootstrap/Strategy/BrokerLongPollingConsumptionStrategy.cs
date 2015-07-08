using System;
using Arch.CMessaging.Client.Core.Lease;
using Arch.CMessaging.Client.Consumer.Engine.Lease;
using Arch.CMessaging.Client.Consumer.Engine.Notifier;
using Arch.CMessaging.Client.Transport.EndPoint;
using Arch.CMessaging.Client.Core.Message;
using Arch.CMessaging.Client.Consumer.Engine.Config;
using Arch.CMessaging.Client.Core.Service;
using Arch.CMessaging.Client.Consumer.Engine.Monitor;
using Arch.CMessaging.Client.Core.Env;
using Arch.CMessaging.Client.Core.Ioc;

namespace Arch.CMessaging.Client.Consumer.Engine.Bootstrap.Strategy
{
    [Named(ServiceType = typeof(IBrokerConsumptionStrategy))]
    public class BrokerLongPollingConsumptionStrategy : IBrokerConsumptionStrategy
    {
        [Inject]
        private ILeaseManager<ConsumerLeaseKey> m_leaseManager;

        [Inject]
        private IConsumerNotifier m_consumerNotifier;

        [Inject]
        private IEndpointManager m_endpointManager;

        [Inject]
        private IEndpointClient m_endpointClient;

        [Inject]
        private IMessageCodec m_messageCodec;

        [Inject]
        private ConsumerConfig m_config;

        [Inject]
        private ISystemClockService m_systemClockService;

        [Inject]
        private IPullMessageResultMonitor m_pullMessageResultMonitor;

        [Inject]
        private IClientEnvironment m_clientEnv;

        public ISubscribeHandle Start(ConsumerContext context, int partitionId)
        {
            return null;
            /*
            try
            {
                int localCachSize = Convert.ToInt32(m_clientEnv.GetConsumerConfig(context.Topic.Name).GetProperty(
                                            "consumer.localcache.size", m_config.DefautlLocalCacheSize));

                int prefetchSize = Convert.ToInt32(m_clientEnv.GetConsumerConfig(context.Topic.Name).GetProperty(
                                           "consumer.localcache.prefetch.threshold.percentage",
                                           m_config.DefaultLocalCachePrefetchThresholdPercentage));

                LongPollingConsumerTask consumerTask = new LongPollingConsumerTask(//
                                                           context, //
                                                           partitionId,//
                                                           localCachSize, //
                                                           prefetchSize,//
                                                           m_systemClockService);

                consumerTask.EndpointClient = m_endpointClient;
                consumerTask.ConsumerNotifier = m_consumerNotifier;
                consumerTask.EndpointManager = m_endpointManager;
                consumerTask.LeaseManager = m_leaseManager;
                consumerTask.MessageCodec = m_messageCodec;
                consumerTask.SystemClockService = m_systemClockService;
                consumerTask.Config = m_config;
                consumerTask.PullMessageResultMonitor = m_pullMessageResultMonitor;
                
                Thread thread = HermesThreadFactory.create(
                                    String.format("LongPollingExecutorThread-%s-%s-%s", context.Topic.Name, partitionId,
                                        context.GroupId), false).newThread(consumerTask);
                thread.start();
                return new BrokerLongPollingSubscribeHandler(consumerTask);
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Start Consumer failed(topic={0}, partition={1}, groupId={2})", context
					.Topic.Name, partitionId, context.GroupId), e);
            }
            */
        }

        /*
        private class BrokerLongPollingSubscribeHandler : ISubscribeHandle
        {

            private LongPollingConsumerTask m_consumerTask;

            public BrokerLongPollingSubscribeHandler(LongPollingConsumerTask consumerTask)
            {
                m_consumerTask = consumerTask;
            }

            public override void close()
            {
                m_consumerTask.close();
            }

        }
        */
    }
}

