using System;
using System.Collections.Generic;
using Freeway.Logging;
using Arch.CMessaging.Client.Core.Pipeline;
using Arch.CMessaging.Client.Consumer.Engine.Config;
using Arch.CMessaging.Client.Core.Service;
using Arch.CMessaging.Client.Core.Env;
using Arch.CMessaging.Client.Consumer.Engine;
using System.Collections.Concurrent;
using Arch.CMessaging.Client.Core.Message;

namespace Arch.CMessaging.Client.Consumer.Engine.Notifier
{
	public class DefaultConsumerNotifier : IConsumerNotifier
	{
		/*private static readonly ILog log = LogManager.GetLogger(typeof(DefaultConsumerNotifier));

		//private ConcurrentMap<Long, Pair<ConsumerContext, ExecutorService>> m_consumerContexs = new ConcurrentHashMap<>();
		private ConcurrentDictionary<long, Pair<ConsumerContext, IExecutor>> m_consumerContexs;

		// @Inject(BuildConstants.CONSUMER)
		private IPipeline<Void> m_pipeline;

		private ConsumerConfig m_config;

		private ISystemClockService m_systemClockService;

		private IClientEnvironment m_clientEnv;
		*/

		public void Register(long correlationId, ConsumerContext context) {
			/*
			try {
				if (log.isDebugEnabled()) {
					log.Debug("Registered(correlationId={}, topic={}, groupId={}, sessionId={})", correlationId, context
						.Topic.Name, context.GroupId, context.SessionId);
				}

				int threadCount = Integer.valueOf(m_clientEnv.getConsumerConfig(context.getTopic().getName()).getProperty(
					"consumer.notifier.threadcount", m_config.getDefaultNotifierThreadCount()));

				m_consumerContexs.putIfAbsent(
					correlationId,
					new Pair<>(context, Executors.newFixedThreadPool(
						threadCount,
						HermesThreadFactory.create(
							String.format("ConsumerNotifier-%s-%s-%s", context.getTopic().getName(),
								context.getGroupId(), correlationId), false))));
			} catch (Exception e) {
				throw new RuntimeException("Register consumer notifier failed", e);
			}
			*/
		}

		public void Deregister(long correlationId) {
			/*
			KeyValuePair<ConsumerContext, ExecutorService> pair = m_consumerContexs.remove(correlationId);
			ConsumerContext context = pair.getKey();
			if (log.isDebugEnabled()) {
				log.Debug("Deregistered(correlationId={}, topic={}, groupId={}, sessionId={})", correlationId, context
					.getTopic().getName(), context.getGroupId(), context.getSessionId());
			}
			pair.getValue().shutdown();
			return;
			*/
		}

		public void MessageReceived(long correlationId, List<IConsumerMessage<Object>> msgs) {
			/*
			KeyValuePair<ConsumerContext, ExecutorService> pair = m_consumerContexs.get(correlationId);
			ConsumerContext context = pair.getKey();
			ExecutorService executorService = pair.getValue();

			executorService.submit(new Runnable() {

				public override void run() {
					try {
						foreach (ConsumerMessage<Object> msg in msgs) {
							if (msg instanceof BrokerConsumerMessage) {
								BrokerConsumerMessage bmsg = (BrokerConsumerMessage) msg;
								bmsg.setCorrelationId(correlationId);
								bmsg.setGroupId(context.getGroupId());
							}
						}

						m_pipeline.put(new Pair<ConsumerContext, List<ConsumerMessage<Object>>>(context, msgs));
					} catch (Exception e) {
						log.error(

							"Exception occurred while calling messageReceived(correlationId={}, topic={}, groupId={}, sessionId={})",
							correlationId, context.getTopic().getName(), context.getGroupId(), context.getSessionId(), e);
					}
				}
			});
			*/

		}

		public ConsumerContext Find(long correlationId) {
			return null;
			/*
			Pair<ConsumerContext, ExecutorService> pair = m_consumerContexs.get(correlationId);
			return pair == null ? null : pair.getKey();
			*/
		}
	}
}

