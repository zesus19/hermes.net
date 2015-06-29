using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Arch.CMessaging.Client.Core.Future;
using Arch.CMessaging.Client.Core.Result;
using Arch.CMessaging.Client.Core.Message;
using Freeway.Logging;
using Arch.CMessaging.Client.Producer.Config;
using Arch.CMessaging.Client.Core.Env;
using Arch.CMessaging.Client.Core.Service;
using Arch.CMessaging.Client.Core.Utils;
using Arch.CMessaging.Client.Core.Collections;
using Arch.CMessaging.Client.Transport.Command;
using System.Diagnostics;
using Arch.CMessaging.Client.Core.Ioc;

namespace Arch.CMessaging.Client.Producer.Sender
{
    public class BrokerMessageSender : AbstractMessageSender
    {
        private Timer endpointSenderScheduler;
        private static readonly ILog log = LogManager.GetLogger(typeof(BrokerMessageSender));

        [Inject]
        private ProducerConfig config;

        [Inject]
        private IClientEnvironment clientEnv;

        [Inject]
        private ISystemClockService systemClockService;

        private ConcurrentDictionary<Pair<string, int>, TaskQueue> taskQueues;

        public BrokerMessageSender()
        {
            this.taskQueues = new ConcurrentDictionary<Pair<string, int>, TaskQueue>();
        }

        public ILog Log { get { return log; } }
        public ProducerConfig Config { get { return config; } }
        public IClientEnvironment ClientEnv { get { return clientEnv; } }
        public ISystemClockService ClockService { get { return systemClockService; } }


        protected override IFuture<SendResult> DoSend(ProducerMessage message)
        {
            var tp = new Pair<string, int>(message.Topic, message.Partition);
            var queueSize = clientEnv.GetGlobalConfig().GetProperty(
                "producer.sender.taskqueue.size", config.DefaultBrokerSenderTaskQueueSize);
            TaskQueue task = null;
            if (!taskQueues.TryGetValue(tp, out task))
            {
                task = new TaskQueue(message.Topic, message.Partition, Convert.ToInt32(queueSize));
                taskQueues.TryAdd(tp, task);
            }

            return task.Submit(message);
        }

        public void Initialize()
        {
            var interval = Convert.ToInt32(clientEnv.GetGlobalConfig().GetProperty(
                "producer.networkio.interval", config.DefaultBrokerSenderNetworkIoCheckIntervalMillis));
            endpointSenderScheduler = new Timer(
                new EndpointSender(taskQueues, interval, this).Run, 
                endpointSenderScheduler, interval, Timeout.Infinite);
        }

        private class TaskQueue
        {
            private string topic;
            private int partition;
            private BlockingQueue<ProducerWorkerContext> queue;
            private ThreadSafe.AtomicReference<SendMessageCommand> cmd;
            
            public TaskQueue(string topic, int partition, int queueSize)
            {
                this.topic = topic;
                this.partition = partition;
                this.queue = new BlockingQueue<ProducerWorkerContext>(queueSize);
            }

            public void Pop()
            {
                cmd.WriteFullFence(null);
            }

            public SendMessageCommand Peek(int size)
            {
                if (cmd.ReadFullFence() == null) cmd.WriteFullFence(CreateSendMessageCommand(size));
                return cmd.ReadFullFence();
            }

            public IFuture<SendResult> Submit(ProducerMessage message)
            {
                var future = SettableFuture<SendResult>.Create();
                queue.Offer(new ProducerWorkerContext(message, future));
                if (message.Callback != null)
                {
                    //add callback
                }
                return future;
            }

            public bool HasTask()
            {
                return cmd.ReadFullFence() != null && queue.Count > 0;
            }

            private SendMessageCommand CreateSendMessageCommand(int size)
            {
                SendMessageCommand command = null;
                IList<ProducerWorkerContext> contexts = new List<ProducerWorkerContext>(size);
                queue.DrainTo(contexts, size);
                if (contexts.Count > 0)
                {
                    command = new SendMessageCommand(topic, partition);
                    foreach (var context in contexts) command.AddMessage(context.Message, context.Future);
                }
                return command;
            }
        }

        private class EndpointSender
        {
            private int checkInterval;
            private ProducerConsumer<SendTask> producer;
            private BrokerMessageSender sender;
            private ConcurrentDictionary<Pair<string, int>, ThreadSafe.Boolean> runnings;
            private ConcurrentDictionary<Pair<string, int>, TaskQueue> taskQueues;
            private const int MAX_TASK_EXEC_CAPACITY = 1000;
            public EndpointSender(
                ConcurrentDictionary<Pair<string, int>, TaskQueue> taskQueues, int checkInterval, BrokerMessageSender sender)
            {
                this.sender = sender;
                this.taskQueues = taskQueues;
                this.checkInterval = checkInterval;
                this.runnings = new ConcurrentDictionary<Pair<string, int>, ThreadSafe.Boolean>();
                this.producer = new ProducerConsumer<SendTask>(MAX_TASK_EXEC_CAPACITY, Convert.ToInt32(
                    sender.ClientEnv.GetGlobalConfig().GetProperty(
                        "producer.networkio.threadcount", sender.Config.DefaultBrokerSenderNetworkIoThreadCount)));
                this.producer.OnConsume += producer_OnConsume;
            }

            void producer_OnConsume(object sender, ConsumeEventArgs e)
            {
                (e.ConsumingItem as SingleConsumingItem<SendTask>).Item.Run();
            }

            public void Run(object state)
            {
                var endpointSenderScheduler = state as Timer;
                try
                {
                    endpointSenderScheduler.Change(Timeout.Infinite, Timeout.Infinite);
                    foreach (var kvp in taskQueues)
                    {
                        var queue = kvp.Value;
                        if (queue.HasTask())
                        {
                            runnings.TryAdd(kvp.Key, new ThreadSafe.Boolean(false));
                            ThreadSafe.Boolean running;
                            if (runnings.TryGetValue(kvp.Key, out running))
                            {
                                if (running.AtomicCompareExchange(true, false))
                                {
                                    producer.Produce(new SendTask(
                                        kvp.Key.Key, kvp.Key.Value, kvp.Value, running, sender));
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    sender.Log.Error(ex);
                }
                finally 
                {
                    endpointSenderScheduler.Change(checkInterval, Timeout.Infinite);
                }
            }
        }

        private class SendTask
        {
            private string topic;
            private int partition;
            private TaskQueue taskQueue;
            private ThreadSafe.Boolean running;
            private BrokerMessageSender sender;
            public SendTask(
                string topic, 
                int partition, 
                TaskQueue taskQueue, 
                ThreadSafe.Boolean running,
                BrokerMessageSender sender)
            {
                this.topic = topic;
                this.partition = partition;
                this.taskQueue = taskQueue;
                this.running = running;
                this.sender = sender;
            }

            public void Run()
            {
                try
                {
                    var batchSize = Convert.ToInt32(
                        sender.ClientEnv.GetGlobalConfig().GetProperty(
                            "producer.sender.batchsize", sender.Config.DefaultBrokerSenderBatchSize));
                    var command = taskQueue.Peek(batchSize);
                    if (command != null && SendMessagesToBroker(command)) taskQueue.Pop();
                }
                catch (Exception ex)
                {
                    sender.Log.Error(ex);
                }
                finally
                {
                    running.AtomicExchange(false);
                }
            }

            private bool SendMessagesToBroker(SendMessageCommand command)
            {
                var brokerAccepted = false;
                var endpoint = sender.EndpointManager.GetEndpoint(topic, partition);
                if (endpoint != null)
                {
                    command.ExpireTime = sender.ClockService.Now() + sender.Config.SendMessageReadResultTimeoutMillis;
                    var future = sender.SendMessageAcceptanceMonitor.Monitor(command.Header.CorrelationId);
                    sender.SendMessageResultMonitor.Monitor(command);
                    var timeout = Convert.ToInt32(sender.ClientEnv
                        .GetGlobalConfig().GetProperty("producer.sender.send.timeout", sender.Config.DefaultBrokerSenderSendTimeoutMillis));
                    sender.EndpointClient.WriteCommand(endpoint, command, timeout);
                    try
                    {
                        brokerAccepted = future.Get(timeout);
                    }
                    catch (Exception ex)
                    {
                        sender.Log.Error(ex);
                        future.Cancel(true);
                    }
                }
                else sender.Log.Debug("no endpoint found, ignore it");
                return brokerAccepted;
            }
        }

        private class ProducerWorkerContext
        {
            public ProducerWorkerContext(ProducerMessage message, SettableFuture<SendResult> future)
            {
                this.Future = future;
                this.Message = message;
            }

            public ProducerMessage Message { get; private set; }
            public SettableFuture<SendResult> Future { get; private set; }
        }
    }
}
