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
using Arch.CMessaging.Client.Core.Collections;
using Arch.CMessaging.Client.Core.MetaService;
using Arch.CMessaging.Client.Core.Message.Retry;

namespace Arch.CMessaging.Client.Consumer.Engine.Bootstrap.Strategy
{
    [Named(ServiceType = typeof(IBrokerConsumptionStrategy))]
    public class BrokerLongPollingConsumptionStrategy : IBrokerConsumptionStrategy
    {
        [Inject]
        private ILeaseManager<ConsumerLeaseKey> LeaseManager;

        [Inject]
        private IConsumerNotifier ConsumerNotifier;

        [Inject]
        private IEndpointManager EndpointManager;

        [Inject]
        private IEndpointClient EndpointClient;

        [Inject]
        private IMessageCodec MessageCodec;

        [Inject]
        private ConsumerConfig Config;

        [Inject]
        private ISystemClockService SystemClockService;

        [Inject]
        private IPullMessageResultMonitor pullMessageResultMonitor;

        [Inject]
        private IClientEnvironment ClientEnv;

        [Inject]
        private IMetaService metaService;

        public ISubscribeHandle Start(ConsumerContext context, int partitionId)
        {
            try
            {
                int localCachSize = Convert.ToInt32(ClientEnv.GetConsumerConfig(context.Topic.Name).GetProperty(
                                            "consumer.localcache.size", Config.DefautlLocalCacheSize));

                int prefetchSize = Convert.ToInt32(ClientEnv.GetConsumerConfig(context.Topic.Name).GetProperty(
                                           "consumer.localcache.prefetch.threshold.percentage",
                                           Config.DefaultLocalCachePrefetchThresholdPercentage));

                IRetryPolicy retryPolicy = metaService.FindRetryPolicyByTopicAndGroup(context.Topic.Name, context.GroupId);
                LongPollingConsumerTask consumerTask = new LongPollingConsumerTask(//
                                                           context, //
                                                           partitionId,//
                                                           localCachSize, //
                                                           prefetchSize,//
                                                           SystemClockService,//
                                                           retryPolicy);

                consumerTask.EndpointClient = EndpointClient;
                consumerTask.ConsumerNotifier = ConsumerNotifier;
                consumerTask.EndpointManager = EndpointManager;
                consumerTask.LeaseManager = LeaseManager;
                consumerTask.MessageCodec = MessageCodec;
                consumerTask.SystemClockService = SystemClockService;
                consumerTask.Config = Config;
                consumerTask.PullMessageResultMonitor = pullMessageResultMonitor;

                ProducerConsumer<LongPollingConsumerTask> fakeThread = new ProducerConsumer<LongPollingConsumerTask>(int.MaxValue);
                fakeThread.OnConsume += StartConsumerTaskLoop;
                fakeThread.Produce(consumerTask);
                
                return new BrokerLongPollingSubscribeHandler(consumerTask);
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Start Consumer failed(topic={0}, partition={1}, groupId={2})", context
					.Topic.Name, partitionId, context.GroupId), e);
            }
        }

        void StartConsumerTaskLoop(object sender, ConsumeEventArgs e)
        {
            LongPollingConsumerTask task = (e.ConsumingItem as SingleConsumingItem<LongPollingConsumerTask>).Item;
            task.Run();
        }

        private class BrokerLongPollingSubscribeHandler : ISubscribeHandle
        {

            private LongPollingConsumerTask ConsumerTask;

            public BrokerLongPollingSubscribeHandler(LongPollingConsumerTask consumerTask)
            {
                ConsumerTask = consumerTask;
            }

            public void Close()
            {
                ConsumerTask.Close();
            }

        }
    }
}

