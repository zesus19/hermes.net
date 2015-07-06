using System;
using Arch.CMessaging.Client.Consumer.Engine.Notifier;
using Arch.CMessaging.Client.Core.Message;
using Arch.CMessaging.Client.Transport.EndPoint;
using Arch.CMessaging.Client.Core.Lease;
using Arch.CMessaging.Client.Core.Service;
using Arch.CMessaging.Client.Consumer.Engine;
using Arch.CMessaging.Client.Consumer.Engine.Lease;
using Arch.CMessaging.Client.Consumer.Engine.Config;
using Arch.CMessaging.Client.Consumer.Engine.Monitor;
using Arch.CMessaging.Client.Core.Bo;
using Freeway.Logging;
using System.Threading;
using Arch.CMessaging.Client.Core.Collections;
using Arch.CMessaging.Client.Core.Utils;

namespace Arch.CMessaging.Client.Consumer.Engine.Bootstrap.Strategy
{
    public class LongPollingConsumerTask
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(LongPollingConsumerTask));

        public IConsumerNotifier ConsumerNotifier { get; set; }

        public IMessageCodec MessageCodec { get; set; }

        public IEndpointClient EndpointManager { get; set; }

        public IEndpointClient EndpointClient { get; set; }

        public ILeaseManager<ConsumerLeaseKey> LeaseManager { get; set; }

        public ISystemClockService SystemClockService;

        public ConsumerConfig Config{ get; set; }

        private ProducerConsumer<Action> m_pullMessageTaskExecutorService;

        private ProducerConsumer<Action> m_renewLeaseTaskExecutorService;

        public IPullMessageResultMonitor PullMessageResultMonitor { get; set; }

        private BlockingQueue<IConsumerMessage> m_msgs;

        private int cacheSize;

        private int localCachePrefetchThreshold;

        private ConsumerContext Context;

        private int PartitionId;

        private volatile bool PullTaskRunning = false;

        private ThreadSafe.AtomicReference<ILease> m_lease = new ThreadSafe.AtomicReference<ILease>(null);

        private volatile bool Closed = false;

        /*
        public LongPollingConsumerTask(ConsumerContext context, int partitionId, int cacheSize, int prefetchThreshold,
                                       ISystemClockService systemClockService)
        {
            Context = context;
            PartitionId = partitionId;
            this.cacheSize = cacheSize;
            this.localCachePrefetchThreshold = prefetchThreshold;
            m_msgs = new BlockingQueue<IConsumerMessage>(cacheSize);
            SystemClockService = systemClockService;

            m_pullMessageTaskExecutorService = new ProducerConsumer<Action>();

            m_renewLeaseTaskExecutorService = new ProducerConsumer<Action>();
        }

        private bool isClosed()
        {
            return Closed;
        }

        public void run()
        {
            log.Info("Consumer started(topic={}, partition={}, groupId={}, sessionId={})", Context.Topic.Name,
                PartitionId, Context.GroupId, Context.SessionId);
            ConsumerLeaseKey key = new ConsumerLeaseKey(new Tpg(Context.Topic.Name, PartitionId,
                                           Context.GroupId), Context.SessionId);
            while (!isClosed())
            {
                try
                {
                    acquireLease(key);

                    if (!isClosed() && m_lease.ReadFullFence() != null && !m_lease.ReadFullFence().IsExpired())
                    {
                        long correlationId = CorrelationIdGenerator.generateCorrelationId();
                        log.Info(string.Format(
                                "Consumer continue consuming(topic={0}, partition={1}, groupId={2}, correlationId={3}, sessionId={4}), since lease acquired",
                                Context.Topic.Name, PartitionId, Context.GroupId, correlationId,
                                Context.SessionId));

                        startConsumingMessages(key, correlationId);

                        log.Info(string.Format(
                                "Consumer pause consuming(topic={0}, partition={1}, groupId={2}, correlationId={3}, sessionId={4}), since lease expired",
                                Context.Topic.Name, PartitionId, Context.GroupId, correlationId,
                                Context.SessionId));
                    }
                }
                catch (Exception e)
                {
                    log.Error(string.Format("Exception occurred in consumer's run method(topic={0}, partition={1}, groupId={2}, sessionId={3})",
                            Context.Topic.Name, PartitionId, Context.GroupId, Context.SessionId), e);
                }
            }

            m_pullMessageTaskExecutorService.Shutdown();
            m_renewLeaseTaskExecutorService.Shutdown();
            log.Info(string.Format("Consumer stopped(topic={0}, partition={1}, groupId={2}, sessionId={3})", Context.Topic.Name,
                    PartitionId, Context.GroupId, Context.SessionId));
        }

        private void startConsumingMessages(ConsumerLeaseKey key, long correlationId)
        {
            ConsumerNotifier.Register(correlationId, Context);

            while (!isClosed() && !m_lease.ReadFullFence().IsExpired())
            {

                try
                {
                    // if leaseRemainingTime < stopConsumerTimeMillsBeforLeaseExpired, stop
                    if (m_lease.ReadFullFence().GetRemainingTime() <= Config.StopConsumerTimeMillsBeforLeaseExpired)
                    {
                        break;
                    }

                    if (m_msgs.Count <= localCachePrefetchThreshold)
                    {
                        schedulePullMessagesTask(correlationId);
                    }

                    if (m_msgs.Count != 0)
                    {
                        consumeMessages(correlationId, cacheSize);
                    }
                    else
                    {
                        Thread.Sleep(Config.NoMessageWaitIntervalMillis);
                    }

                }
                catch (Exception e)
                {
                    log.Error(string.Format("Exception occurred while consuming message(topic={0}, partition={1}, groupId={2}, sessionId={3})",
                            Context.Topic.Name, PartitionId, Context.GroupId, Context.SessionId), e);
                }
            }

            // consume all remaining messages
            if (m_msgs.Count != 0)
            {
                consumeMessages(correlationId, 0);
            }

            m_consumerNotifier.Reregister(correlationId);
            m_lease.WriteFullFence(null);
        }

		private void scheduleRenewLeaseTask(ConsumerLeaseKey key, long delay) {
            m_renewLeaseTaskExecutorService.schedule(delegate {

				public void run() {
					if (isClosed()) {
						return;
					}

					Lease lease = m_lease.get();
					if (lease != null) {
						if (lease.getRemainingTime() > 0) {
							LeaseAcquireResponse response = m_leaseManager.tryRenewLease(key, lease);
							if (response != null && response.isAcquired()) {
								lease.setExpireTime(response.getLease().getExpireTime());
								scheduleRenewLeaseTask(key,
									lease.getRemainingTime() - m_config.getRenewLeaseTimeMillisBeforeExpired());
								if (log.isDebugEnabled()) {
									log.debug("Consumer renew lease success(topic={}, partition={}, groupId={}, sessionId={})",
										Context.Topic.Name, PartitionId, Context.GroupId,
										Context.SessionId);
								}
							} else {
								if (response != null && response.getNextTryTime() > 0) {
									scheduleRenewLeaseTask(key, response.getNextTryTime() - m_systemClockService.now());
								} else {
									scheduleRenewLeaseTask(key, m_config.getDefaultLeaseRenewDelayMillis());
								}

								if (log.isDebugEnabled()) {
									log.debug(
										"Unable to renew consumer lease(topic={}, partition={}, groupId={}, sessionId={}), ignore it",
										Context.Topic.Name, PartitionId, Context.GroupId,
										Context.SessionId);
								}
							}
						}
					}
				}
			}, delay, TimeUnit.MILLISECONDS);
		}

		private void acquireLease(ConsumerLeaseKey key) {
			long nextTryTime = m_systemClockService.now();
			while (!isClosed() && !Thread.currentThread().isInterrupted()) {
				try {
					while (true) {
						if (!isClosed() && !Thread.currentThread().isInterrupted()) {
							if (nextTryTime > m_systemClockService.now()) {
								LockSupport.parkUntil(nextTryTime);
							} else {
								break;
							}
						} else {
							return;
						}
					}

					if (isClosed()) {
						return;
					}

					LeaseAcquireResponse response = m_leaseManager.tryAcquireLease(key);

					if (response != null && response.isAcquired() && !response.getLease().isExpired()) {
						m_lease.set(response.getLease());
						scheduleRenewLeaseTask(key,
							m_lease.get().getRemainingTime() - m_config.getRenewLeaseTimeMillisBeforeExpired());

						if (log.isDebugEnabled()) {
							log.debug(
								"Acquire consumer lease success(topic={}, partition={}, groupId={}, sessionId={}, leaseId={}, expireTime={})",
								Context.Topic.Name, PartitionId, Context.GroupId,
								Context.SessionId, response.getLease().getId(), new Date(response.getLease()
									.getExpireTime()));
						}
						return;
					} else {
						if (response != null && response.getNextTryTime() > 0) {
							nextTryTime = response.getNextTryTime();
						} else {
							nextTryTime = m_systemClockService.now() + m_config.getDefaultLeaseAcquireDelayMillis();
						}

						if (log.isDebugEnabled()) {
							log.debug(
								"Unable to acquire consumer lease(topic={}, partition={}, groupId={}, sessionId={}), ignore it",
								Context.Topic.Name, PartitionId, Context.GroupId, Context.SessionId);
						}
					}
				} catch (Exception e) {
					log.error("Exception occurred while acquiring lease(topic={}, partition={}, groupId={}, sessionId={})",
						Context.Topic.Name, PartitionId, Context.GroupId, Context.SessionId, e);
				}
			}
		}

		private void consumeMessages(long correlationId, int maxItems) {
			List<ConsumerMessage<?>> msgs = new ArrayList<>(maxItems <= 0 ? 100 : maxItems);

			if (maxItems <= 0) {
				m_msgs.drainTo(msgs);
			} else {
				m_msgs.drainTo(msgs, maxItems);
			}

			m_consumerNotifier.messageReceived(correlationId, msgs);
		}

		@SuppressWarnings("rawtypes")
		private List<ConsumerMessage<?>> decodeBatches(List<TppConsumerMessageBatch> batches, Class bodyClazz,
			Channel channel) {
			List<ConsumerMessage<?>> msgs = new ArrayList<>();
			for (TppConsumerMessageBatch batch : batches) {
				List<MessageMeta> msgMetas = batch.getMessageMetas();
				ByteBuf batchData = batch.getData();

				int partition = batch.getPartition();

				for (int j = 0; j < msgMetas.size(); j++) {
					BaseConsumerMessage baseMsg = m_messageCodec.decode(batch.Topic, batchData, bodyClazz);
					BrokerConsumerMessage brokerMsg = new BrokerConsumerMessage(baseMsg);
					MessageMeta messageMeta = msgMetas.get(j);
					brokerMsg.setPartition(partition);
					brokerMsg.setPriority(messageMeta.getPriority() == 0 ? true : false);
					brokerMsg.setResend(messageMeta.isResend());
					brokerMsg.setChannel(channel);
					brokerMsg.setMsgSeq(messageMeta.getId());

					msgs.add(brokerMsg);
				}
			}

			return msgs;
		}

		private void schedulePullMessagesTask(long correlationId) {
			if (!isClosed() && m_pullTaskRunning.compareAndSet(false, true)) {
				m_pullMessageTaskExecutorService.submit(new PullMessagesTask(correlationId));
			}
		}

		private class PullMessagesTask implements Runnable {
			private long m_correlationId;

			public PullMessagesTask(long correlationId) {
				m_correlationId = correlationId;
			}

			@Override
			public void run() {
				try {
					if (isClosed() || m_msgs.size() > m_localCachePrefetchThreshold) {
						return;
					}

					Endpoint endpoint = m_endpointManager.getEndpoint(Context.Topic.Name, PartitionId);

					if (endpoint == null) {
						log.warn("No endpoint found for topic {} partition {}, will retry later",
							Context.Topic.Name, PartitionId);
						TimeUnit.MILLISECONDS.sleep(m_config.getNoEndpointWaitIntervalMillis());
						return;
					}

					SettableFuture<PullMessageResultCommand> future = SettableFuture.create();

					Lease lease = m_lease.get();
					if (lease != null) {
						long timeout = lease.getRemainingTime();

						if (timeout > 0) {
							PullMessageCommand cmd = new PullMessageCommand(Context.Topic.Name, PartitionId,
								Context.GroupId, m_cacheSize - m_msgs.size(), m_systemClockService.now() + timeout
								- 500L);

							cmd.getHeader().setCorrelationId(m_correlationId);
							cmd.setFuture(future);

							PullMessageResultCommand ack = null;

							try {
								m_pullMessageResultMonitor.monitor(cmd);
								m_endpointClient.writeCommand(endpoint, cmd, timeout, TimeUnit.MILLISECONDS);

								ack = future.get(timeout, TimeUnit.MILLISECONDS);

								if (ack == null) {
									return;
								}
								List<TppConsumerMessageBatch> batches = ack.getBatches();
								if (batches != null && !batches.isEmpty()) {
									ConsumerContext context = m_consumerNotifier.find(m_correlationId);
									if (context != null) {
										Class<?> bodyClazz = context.getMessageClazz();

										List<ConsumerMessage<?>> msgs = decodeBatches(batches, bodyClazz, ack.getChannel());
										m_msgs.addAll(msgs);
									} else {
										log.warn("Can not find consumerContext(topic={}, partition={}, groupId={}, sessionId={})",
											Context.Topic.Name, PartitionId, Context.GroupId,
											Context.SessionId);
									}
								}
							} finally {
								if (ack != null) {
									ack.release();
								}
							}
						}
					}
				} catch (TimeoutException e) {
					// ignore
				} catch (Exception e) {
					log.warn("Exception occurred while pulling message(topic={}, partition={}, groupId={}, sessionId={}).",
						Context.Topic.Name, PartitionId, Context.GroupId, Context.SessionId, e);
				} finally {
					m_pullTaskRunning.set(false);
				}
			}

		}

		public void close() {
			m_closed.set(true);
		}*/
    }
}

