using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Arch.CMessaging.Client.API;
using Arch.CMessaging.Client.Consumer.Api;
using Arch.CMessaging.Client.Core.Collections;
using Arch.CMessaging.Client.Core.Message;
using Arch.CMessaging.Client.Core.Message.Payload;
using Arch.CMessaging.Client.Core.Utils;
using Arch.CMessaging.Client.Event;
using Arch.CMessaging.Client.Impl.Consumer;
using Arch.CMessaging.Core.Util;
using Arch.CMessaging.Client.Impl.Consumer.Check;
using Arch.CMessaging.Core.Log;
using Freeway.Logging;

namespace Arch.CMessaging.Client.Impl.Consumer
{
    public class HermesConsumer : ITopicConsumer, IQueueConsumer, IBlockingSupport, IDisposed
    {
        private string bindTopic = string.Empty;
        private string exchangeName = string.Empty;
        private string queueName = string.Empty;
        private bool isDisposed;
        private event ConsumerCallbackEventHandler callback;
        private Arch.CMessaging.Core.Util.ThreadSafe.Boolean notStarted;
        private Arch.CMessaging.Core.Util.ThreadSafe.Boolean hasRegistered;
        private static Freeway.Logging.ILog log = LogManager.GetLogger(typeof(HermesConsumer));

        public HermesConsumer()
        {
            this.notStarted = new Arch.CMessaging.Core.Util.ThreadSafe.Boolean(true);
            this.hasRegistered = new CMessaging.Core.Util.ThreadSafe.Boolean(false); 
        }

        public event ConsumerCallbackEventHandler Callback
        {
            add
            {
                if (hasRegistered.AtomicCompareExchange(true, false))
                {
                    callback += value;
                }
            }
            remove
            {
                if (hasRegistered.AtomicCompareExchange(false, true))
                {
                    callback -= value;
                }
            }
        }

        public BlockingQueue<ConsumerCallbackEventArgs> BlockingQ { get; set; }

        internal void InvokeCallback(ConsumerCallbackEventArgs args)
        {
            if (callback != null) callback(this, args);
        }

        #region ITopicConsumer Members

        public string HeaderFilter { get { return string.Empty; } }

        public void TopicBind(string topic, string exchangeName, string queueName = null)
        {
            Arch.CMessaging.Core.Util.Guard.ArgumentNotNullOrEmpty(Identifier, "Identifier");
            Arch.CMessaging.Core.Util.Guard.ArgumentNotNullOrEmpty(topic, "topic");
            Arch.CMessaging.Core.Util.Guard.ArgumentNotNullOrEmpty(exchangeName, "exchangeName");

            if (!this.bindTopic.Equals(topic, StringComparison.OrdinalIgnoreCase))
            {
                TopicCheck.Check(topic);
                this.bindTopic = topic;
            }
            this.exchangeName = exchangeName;
            this.queueName = queueName;
            if(!string.IsNullOrEmpty(queueName))
            {
                queueName = new HexStringConverter()
                    .ToString(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(topic + exchangeName + Identifier)));
            }
        }

        #endregion

        #region IMessageConsumer Members

        public uint BatchSize { get; set; }

        public uint ReceiveTimeout { get; set; }

        public string Identifier { get; set; }

        public void Consume()
        {
            StartBlockConsume();
            this.InvokeCallback(BlockingQ.Take());
        }

        public IMessage ConsumeOne()
        {
            StartBlockConsume();
            return BlockingQ.Take().Message;
        }

        public void ConsumeAsync(int maxThread = 1, bool autoAck = true)
        {
            if (notStarted.AtomicCompareExchange(false, true))
            {
                var tuple = GetTopicAndGroup();
                Arch.CMessaging.Client.Consumer.Consumer.GetInstance()
                    .Start(tuple.Item1, tuple.Item2, new AsyncCMessageListener(this, tuple.Item2));
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            isDisposed = true;
        }

        #endregion

        #region IQueueConsumer Members

        public void QueueBind(string exchangeName)
        {
            this.exchangeName = exchangeName;
        }

        #endregion

        internal void StartBlockConsume()
        {
            if (notStarted.AtomicCompareExchange(false, true))
            {
                var tuple = GetTopicAndGroup();
                Arch.CMessaging.Client.Consumer.Consumer.GetInstance()
                    .Start(tuple.Item1, tuple.Item2, new BlockCMessageListener(this, tuple.Item2, BlockingQ));
            }
        }

        private Tuple<string, string> GetTopicAndGroup()
        {
            var topic = string.Empty;
            var groupId = string.Empty;
            var client = Arch.CMessaging.Client.Consumer.Consumer.GetInstance();
            if (!string.IsNullOrEmpty(bindTopic)) 
                topic = string.Format("{0}.{1}", this.exchangeName, bindTopic);
            else 
                topic = exchangeName;
            groupId = string.Format("{0}.{1}", Identifier,
                string.IsNullOrEmpty(queueName) ? "_" : queueName);

            return new Tuple<string, string>(topic, groupId);
        }

        private  abstract class AbstractCMessageListener : BaseMessageListener
        {
            private HermesConsumer consumer;
            protected AbstractCMessageListener(HermesConsumer consumer, string groupId)
                : base(groupId)
            {
                this.consumer = consumer;
            }

            protected override void OnMessage(IConsumerMessage msg)
            {
                var reader = new HermesMessageReader(msg);
                var args = new ConsumerCallbackEventArgs(reader);
                using (args.Message = new HermesMessage(reader))
                {
                    try
                    {
                        args.Message.Acks = CMessaging.Core.Content.AckMode.Ack;
                        OnHandleMessage(args);
                    }
                    catch (Exception ex)
                    {
                        args.Message.Acks = CMessaging.Core.Content.AckMode.Nack;
                        log.Error(ex);
                    }
                }
            }

            public override Type MessageType()
            {
                return typeof(RawMessage);
            }

            protected HermesConsumer Consumer { get { return consumer; } }

            protected abstract void OnHandleMessage(ConsumerCallbackEventArgs args);
        }
        
        private class AsyncCMessageListener : AbstractCMessageListener 
        {
            public AsyncCMessageListener(HermesConsumer consumer, string groupId)
                : base(consumer, groupId) 
            {
                
            }

            protected override void OnHandleMessage(ConsumerCallbackEventArgs args)
            {
                Consumer.InvokeCallback(args);
            }
        }

        private class BlockCMessageListener : AbstractCMessageListener
        {
            private BlockingQueue<ConsumerCallbackEventArgs> blockingQ;
            public BlockCMessageListener(
                HermesConsumer consumer, string groupId, BlockingQueue<ConsumerCallbackEventArgs> blockingQ)
                : base(consumer, groupId) 
            {

                this.blockingQ = blockingQ;
            }

            protected override void OnHandleMessage(ConsumerCallbackEventArgs args)
            {
                blockingQ.Put(args, 90 * 1000);
            }
        }

        #region IDisposed Members

        public bool IsDispose { get { return isDisposed; } }

        #endregion
    }
}
