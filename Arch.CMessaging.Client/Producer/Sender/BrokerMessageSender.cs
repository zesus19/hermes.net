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
using Arch.CMessaging.Client.MetaEntity.Entity;
using Arch.CMessaging.Client.Transport.EndPoint;
using Arch.CMessaging.Client.Core.Message.Partition;
using Arch.CMessaging.Client.Core.MetaService;
using Arch.CMessaging.Client.Producer.Monitor;
using Arch.CMessaging.Client.Core.Schedule;
using Arch.CMessaging.Client.Core.Exceptions;

namespace Arch.CMessaging.Client.Producer.Sender
{
	// TODO extends AbstractMessageSender when IoC supports inject to base class
	[Named (ServiceType = typeof(IMessageSender), ServiceName = Endpoint.BROKER)]
	public class BrokerMessageSender : IMessageSender, IInitializable // : AbstractMessageSender
	{
		public Timer endpointSenderScheduler{ get; private set; }

        public ISchedulePolicy SchedulePolicy { get; private set;}

		private static readonly ILog log = LogManager.GetLogger (typeof(BrokerMessageSender));

		[Inject]
		private ProducerConfig config;

		[Inject]
		private IClientEnvironment clientEnv;

		[Inject]
		private ISystemClockService systemClockService;

		// copy from AbstractMessageSender cause IoC not support inject in base class [start]
		[Inject]
		private IEndpointManager endpointManager;


		[Inject]
		private IEndpointClient endpointClient;

		[Inject]
		private IPartitioningStrategy partitioningAlgo;

		[Inject]
		private IMetaService metaService;


		[Inject]
		private ISendMessageAcceptanceMonitor messageAcceptanceMonitor;

		[Inject]
		private ISendMessageResultMonitor messageResultMonitor;

		public IEndpointManager EndpointManager { get { return endpointManager; } }

		public IEndpointClient EndpointClient { get { return endpointClient; } }

		public IMetaService MetaService { get { return metaService; } }

		public ISendMessageAcceptanceMonitor SendMessageAcceptanceMonitor { get { return messageAcceptanceMonitor; } }

		public ISendMessageResultMonitor SendMessageResultMonitor { get { return messageResultMonitor; } }

		#region IMessageSender Members

		public IFuture<SendResult> Send (ProducerMessage message)
		{
			PreSend (message);
			return DoSend (message);
		}

		#endregion

		//protected abstract IFuture<SendResult> DoSend(ProducerMessage message);
		protected void PreSend (ProducerMessage message)
		{

			var partitionNo = partitioningAlgo.ComputePartitionNo (message.PartitionKey, metaService.ListPartitionsByTopic (message.Topic).Count);
			message.Partition = partitionNo;
		}
		// copy from AbstractMessageSender cause IoC not support inject in base class [end]

		private ConcurrentDictionary<Pair<string, int>, TaskQueue> taskQueues;
		private ProducerConsumer<FutureCallbackItem<SendResult>> callbackExecutor;

		public BrokerMessageSender ()
		{
			this.taskQueues = new ConcurrentDictionary<Pair<string, int>, TaskQueue> ();
		}

		public ILog Log { get { return log; } }

		public ProducerConfig Config { get { return config; } }

		public IClientEnvironment ClientEnv { get { return clientEnv; } }

		public ISystemClockService ClockService { get { return systemClockService; } }


		protected IFuture<SendResult> DoSend (ProducerMessage message)
		{
			var tp = new Pair<string, int> (message.Topic, message.Partition);
			var queueSize = clientEnv.GetGlobalConfig ().GetProperty (
				                "producer.sender.taskqueue.size", config.DefaultBrokerSenderTaskQueueSize);
			TaskQueue task = null;
			if (!taskQueues.TryGetValue (tp, out task)) {
				task = new TaskQueue (message.Topic, message.Partition, Convert.ToInt32 (queueSize), this);
				taskQueues.TryAdd (tp, task);
			}

			return task.Submit (message);
		} 

		public void Initialize ()
		{
            int checkIntervalBase = Convert.ToInt32(clientEnv.GetGlobalConfig().GetProperty(
                "producer.networkio.interval.base", config.DefaultBrokerSenderNetworkIoCheckIntervalBaseMillis));
            int checkIntervalMax = Convert.ToInt32(clientEnv.GetGlobalConfig().GetProperty(
                "producer.networkio.interval.max", config.DefaultBrokerSenderNetworkIoCheckIntervalMaxMillis));

            SchedulePolicy = new ExponentialSchedulePolicy(checkIntervalBase, checkIntervalMax);

			endpointSenderScheduler = new Timer (
                new EndpointSender (taskQueues, checkIntervalBase, this).Run, 
                this, checkIntervalBase, Timeout.Infinite);
			this.callbackExecutor = new ProducerConsumer<FutureCallbackItem<SendResult>> (int.MaxValue,
				Convert.ToInt32 (clientEnv.GetGlobalConfig ().GetProperty ("producer.callback.threadcount", config.DefaultProducerCallbackThreadCount)));
			this.callbackExecutor.OnConsume += callbackExecutor_OnConsume;
		}

		void callbackExecutor_OnConsume (object sender, ConsumeEventArgs e)
		{
			var callbackItem = e.ConsumingItem as SingleConsumingItem<FutureCallbackItem<SendResult>>;
			try {
				var result = callbackItem.Item.Item;
				if (result != null) {
					if (result is Exception)
						callbackItem.Item.Callback.OnFailure (result as Exception);
					else
						callbackItem.Item.Callback.OnSuccess (result as SendResult);
				}
			} catch (Exception ex) {
				Log.Error (ex);
			}
		}

		private class TaskQueue
		{
			private string topic;
			private int partition;
			private BrokerMessageSender sender;
			private BlockingQueue<ProducerWorkerContext> queue;
			private ThreadSafe.AtomicReference<SendMessageCommand> cmd;

			public TaskQueue (string topic, int partition, int queueSize, BrokerMessageSender sender)
			{
				this.topic = topic;
				this.sender = sender;
				this.partition = partition;
				this.queue = new BlockingQueue<ProducerWorkerContext> (queueSize);
				this.cmd = new ThreadSafe.AtomicReference<SendMessageCommand> (null);
			}

            public void Push (SendMessageCommand command)
			{
				cmd.WriteFullFence (command);
			}

			public SendMessageCommand Pop (int size)
			{
				if (cmd.ReadFullFence () == null)
					return CreateSendMessageCommand (size);
                return cmd.AtomicExchange(null);
			}

			public IFuture<SendResult> Submit (ProducerMessage message)
			{
				var future = SettableFuture<SendResult>.Create ();

				if (message.Callback != null) {
					Futures<SendResult>.AddCallback (future, new ProducerMessageCallback (message), sender.callbackExecutor);
				}

                if (!queue.Offer(new ProducerWorkerContext(message, future)))
                {
                    string warning = "Producer task queue is full, will drop this message.";
                    log.Warn(warning);
                    MessageSendException throwable = new MessageSendException(warning);
                    future.SetException(throwable);
                }

				return future;
			}

			public bool HasTask ()
			{
				return cmd.ReadFullFence () != null || queue.Count > 0;
			}

			private SendMessageCommand CreateSendMessageCommand (int size)
			{
				SendMessageCommand command = null;
				IList<ProducerWorkerContext> contexts = new List<ProducerWorkerContext> (size);
				queue.DrainTo (contexts, size);
				if (contexts.Count > 0) {
					command = new SendMessageCommand (topic, partition);
					foreach (var context in contexts)
						command.AddMessage (context.Message, context.Future);
				}
				return command;
			}
		}

		private class ProducerMessageCallback : IFutureCallback<SendResult>
		{
			private ProducerMessage message;

			public ProducerMessageCallback (ProducerMessage message)
			{
				this.message = message;
			}

			#region IFutureCallback<SendResult> Members

			public void OnSuccess (SendResult success)
			{
				message.Callback.OnSuccess (success);
			}

			public void OnFailure (Exception ex)
			{
				message.Callback.OnFailure (ex);
			}

			#endregion
		}

		private class EndpointSender
		{
			private int checkInterval;
			private ProducerConsumer<SendTask> producer;
			private BrokerMessageSender sender;
			private ConcurrentDictionary<Pair<string, int>, ThreadSafe.Boolean> runnings;
			private ConcurrentDictionary<Pair<string, int>, TaskQueue> taskQueues;
			private const int MAX_TASK_EXEC_CAPACITY = 1000;

			public EndpointSender (
				ConcurrentDictionary<Pair<string, int>, TaskQueue> taskQueues, int checkInterval, BrokerMessageSender sender)
			{
				this.sender = sender;
				this.taskQueues = taskQueues;
				this.checkInterval = checkInterval;
				this.runnings = new ConcurrentDictionary<Pair<string, int>, ThreadSafe.Boolean> ();
				this.producer = new ProducerConsumer<SendTask> (MAX_TASK_EXEC_CAPACITY, Convert.ToInt32 (
					sender.ClientEnv.GetGlobalConfig ().GetProperty (
						"producer.networkio.threadcount", sender.Config.DefaultBrokerSenderNetworkIoThreadCount)));
				this.producer.OnConsume += producer_OnConsume;
			}

			void producer_OnConsume (object sender, ConsumeEventArgs e)
			{
				(e.ConsumingItem as SingleConsumingItem<SendTask>).Item.Run ();
			}

			public void Run (object state)
			{
				BrokerMessageSender sender = state as BrokerMessageSender;
				var endpointSenderScheduler = sender.endpointSenderScheduler;
                bool hasTask = false;
                try {
					endpointSenderScheduler.Change (Timeout.Infinite, Timeout.Infinite);
					foreach (var kvp in taskQueues) {
						var queue = kvp.Value;
						if (queue.HasTask ()) {
                            hasTask = true;
                            ThreadSafe.Boolean running;
                            if (!runnings.TryGetValue(kvp.Key, out running))
                            {
                                running = new ThreadSafe.Boolean(false);
                                runnings[kvp.Key] = running;
                            }
							if (running.AtomicCompareExchange (true, false)) 
                            {
								producer.Produce (new SendTask (
									kvp.Key.Key, kvp.Key.Value, kvp.Value, running, sender));
							}
						}
					}
				} catch (Exception ex) {
					sender.Log.Error (ex);
				} finally {
                    if (hasTask)
                    {
                        sender.SchedulePolicy.Succeess();
                    }
                    endpointSenderScheduler.Change (sender.SchedulePolicy.Fail(false), Timeout.Infinite);
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

			public SendTask (
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

			public void Run ()
			{
				try {
					var batchSize = Convert.ToInt32 (
						                sender.ClientEnv.GetGlobalConfig ().GetProperty (
							                "producer.sender.batchsize", sender.Config.DefaultBrokerSenderBatchSize));
					
                    var command = taskQueue.Pop (batchSize);

                    if (command != null)
                    {
                        if(!SendMessagesToBroker (command))
                        {
						    taskQueue.Push (command);
                        }
                    }
				} catch (Exception ex) {
					sender.Log.Error (ex);
				} finally {
					running.AtomicExchange (false);
				}
			}

			private bool SendMessagesToBroker (SendMessageCommand command)
			{
				var brokerAccepted = false;
				var endpoint = sender.EndpointManager.GetEndpoint (topic, partition);
				if (endpoint != null) {
					command.ExpireTime = sender.ClockService.Now () + sender.Config.SendMessageReadResultTimeoutMillis;
					var future = sender.SendMessageAcceptanceMonitor.Monitor (command.Header.CorrelationId);
					sender.SendMessageResultMonitor.Monitor (command);
					int timeout = sender.Config.DefaultBrokerSenderSendTimeoutMillis;
					sender.EndpointClient.WriteCommand (endpoint, command, timeout);
					try {
						brokerAccepted = future.Get (timeout);
					} catch (Exception ex) {
						sender.Log.Error (ex);
						future.Cancel (true);
					}
				} else
					sender.Log.Debug ("no endpoint found, ignore it");
				return brokerAccepted;
			}
		}

		private class ProducerWorkerContext
		{
			public ProducerWorkerContext (ProducerMessage message, SettableFuture<SendResult> future)
			{
				this.Future = future;
				this.Message = message;
			}

			public ProducerMessage Message { get; private set; }

			public SettableFuture<SendResult> Future { get; private set; }
		}
	}
}
