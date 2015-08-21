using System.Collections.Concurrent;
using Arch.CMessaging.Client.API;
using Arch.CMessaging.Core.Util;
using System;
using System.IO;

namespace Arch.CMessaging.Client.Impl.Consumer
{
    /// <summary>
    /// 用于快速生成Consumer的工厂
    /// <remarks>
    /// 这是一个简化的过程，如果需要处理异常，内存控制，限流等操作
    /// 请参考使用<see cref="IMessageChannelFactory"/>
    /// </remarks>
    /// </summary>
    public class ConsumerFactory : IConsumerFactory
    {
        ConcurrentDictionary<string, IMessageConsumer> consumerCache = new ConcurrentDictionary<string, IMessageConsumer>();
        private static ConsumerFactory factory;
        private static object lockObject = new object();
        public static ConsumerFactory Instance
        {
            get
            {
                if (factory == null)
                {
                    lock (lockObject)
                    {
                        if (factory == null)
                        {
                            factory = new ConsumerFactory();
                        }
                    }
                }
                return factory;
            }
        }

        internal ConsumerChannel Channel { get; private set; }
        public ConsumerFactory()
        {
            ObjectFactoryLifetimeManager.Instance.Register();
            Channel = new ChannelFactory().CreateChannel<ConsumerChannel>(new DefaultMessageChannelConfigurator());
        }

        /// <summary>
        /// 快速生成一个基于Topic语义消费的Consumer
        /// </summary>
        /// <param name="topic">订阅主题</param>
        /// <param name="exchangeName">Exchange实例名</param>
        /// <param name="identifier">标识Consumer的身份</param>
        /// <returns><see cref="ITopicConsumer"/></returns>
        public ITopicConsumer CreateAsTopic(string topic, string exchangeName , string identifier)
        {
            Guard.ArgumentNotNullOrEmpty(topic, "topic");
            Guard.ArgumentNotNullOrEmpty(exchangeName, "exchangeName");
            Guard.ArgumentNotNullOrEmpty(identifier, "identifier");
            topic = topic.Trim();
            exchangeName = exchangeName.Trim();
            identifier = identifier.Trim();

            var key = string.Format("{0}_{1}_{2}_TOPIC", exchangeName, topic, identifier);
            IMessageConsumer messageConsumer;
            if (consumerCache.TryGetValue(key, out messageConsumer))
            {
                var disposableConsumer = messageConsumer as IDisposed;
                if (disposableConsumer != null)
                {
                    if (disposableConsumer.IsDispose) consumerCache.TryRemove(key, out messageConsumer);
                }
                else
                {
                    var topicConsumer = messageConsumer as ITopicConsumer;
                    if (topicConsumer != null) return topicConsumer;
                }
            }

            ITopicConsumer consumer = null;
            var flag = ConfigUtil.Instance.ConsumerRunAs;
            if (flag == ConsumerFlag.Both)
            {
                consumer = new TwoWayConsumer(Channel.CreateConsumer<TopicConsumer>(), new HermesConsumer());
            }
            else
            {
                if ((flag & ConsumerFlag.Hermes) == ConsumerFlag.Hermes) consumer = new HermesConsumer();
                else consumer = Channel.CreateConsumer<TopicConsumer>();
            }

            consumer.Identifier = identifier;
            consumer.TopicBind(topic, exchangeName);
            consumerCache.TryAdd(key, consumer);
            return consumer;
        }

        /// <summary>
        /// 快速生成一个基于Queue语义消费的Consumer
        /// </summary>
        /// <param name="exchangeName">Exchange实例名</param>
        /// <param name="identifier">标识Consumer的身份</param>
        /// <returns><see cref="IQueueConsumer"/></returns>
        public IQueueConsumer CreateAsQueue(string exchangeName, string identifier)
        {
            Guard.ArgumentNotNullOrEmpty(exchangeName, "exchangeName");
            Guard.ArgumentNotNullOrEmpty(identifier, "identifier");

            exchangeName = exchangeName.Trim();
            identifier = identifier.Trim();

            var key = string.Format("{0}_{1}_QUEUE", exchangeName, identifier);
            IMessageConsumer messageConsumer;
            if (consumerCache.TryGetValue(key, out messageConsumer))
            {
                var disposableConsumer = messageConsumer as IDisposed;
                if (disposableConsumer != null)
                {
                    if (disposableConsumer.IsDispose) consumerCache.TryRemove(key, out messageConsumer);
                }
                else
                {
                    var queueConsumer = messageConsumer as IQueueConsumer;
                    if (queueConsumer != null) return queueConsumer;
                }
            }
            IQueueConsumer consumer = null;
            var flag = ConfigUtil.Instance.ConsumerRunAs;
            if (flag == ConsumerFlag.Both)
            {
                consumer = new TwoWayConsumer(Channel.CreateConsumer<QueueConsumer>(), new HermesConsumer());
            }
            else
            {
                if ((flag & ConsumerFlag.Hermes) == ConsumerFlag.Hermes) consumer = new HermesConsumer();
                else consumer = Channel.CreateConsumer<QueueConsumer>();
            }

            consumer.Identifier = identifier;
            consumer.QueueBind(exchangeName);
            consumerCache.TryAdd(key, consumer);
            return consumer;


            //AbstractConsumer messageConsumer;
            //if (consumerCache.TryGetValue(key, out messageConsumer))
            //{
            //    if (!messageConsumer.IsDispose)
            //    {
            //        var queueConsumer = messageConsumer as IQueueConsumer;
            //        if (queueConsumer != null) return queueConsumer;
            //    }
            //    consumerCache.TryRemove(key, out messageConsumer);
            //}

            //var consumer = Channel.CreateConsumer<QueueConsumer>();
            //consumer.Identifier = identifier;
            //consumer.QueueBind(exchangeName);
            //consumerCache.TryAdd(key, consumer);
        }

        /// <summary>
        /// 快速生成一个基于DeadLetter语义消费的Consumer
        /// </summary>
        /// <param name="topic">订阅主题</param>
        /// <param name="exchangeName">Exchange实例名</param>
        /// <param name="identifier">标识Consumer的身份</param>
        /// <returns><see cref="IQueueConsumer"/></returns>
        public IDeadLetterConsumer CreateAsDeadLetter(string topic, string exchangeName, string identifier)
        {
            Guard.ArgumentNotNullOrEmpty(exchangeName, "exchangeName");
            Guard.ArgumentNotNullOrEmpty(identifier, "identifier");
            exchangeName = exchangeName.Trim();
            identifier = identifier.Trim();

            var key = string.Format("{0}_{1}_{2}_DEAD", exchangeName, identifier,string.IsNullOrEmpty(topic)?"":topic);
            IMessageConsumer messageConsumer;
            if (consumerCache.TryGetValue(key, out messageConsumer))
            {
                var disposableConsumer = messageConsumer as IDisposed;
                if (!disposableConsumer.IsDispose)
                {
                    var deadletterConsumer = messageConsumer as IDeadLetterConsumer;
                    if (deadletterConsumer != null) return deadletterConsumer;
                }
                consumerCache.TryRemove(key, out messageConsumer);
            }
            var consumer = Channel.CreateConsumer<DeadLetterConsumer>();
            consumer.Identifier = identifier;
            consumer.DeadLetterBind(exchangeName, topic);
            consumerCache.TryAdd(key, consumer);
            return consumer;
        }
    }
}
