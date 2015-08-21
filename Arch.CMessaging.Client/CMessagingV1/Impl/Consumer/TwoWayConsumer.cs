using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arch.CMessaging.Client.API;
using Arch.CMessaging.Client.Core.Collections;
using Arch.CMessaging.Client.Event;

namespace Arch.CMessaging.Client.Impl.Consumer
{
    public class TwoWayConsumer : ITopicConsumer, IQueueConsumer, IBlockingSupport, IDisposed
    {
        private AbstractConsumer cmessagingConsumer;
        private HermesConsumer hermesConsumer;
        private BlockingQueue<ConsumerCallbackEventArgs> blockingQ;
        private event ConsumerCallbackEventHandler callback;
        private bool isDisposed;
        public TwoWayConsumer(AbstractConsumer cmessagingConsumer, HermesConsumer hermesConsumer)
        {
            this.hermesConsumer = hermesConsumer;
            this.cmessagingConsumer = cmessagingConsumer;
        }

        #region ITopicConsumer Members

        public string HeaderFilter { get { return null; } }

        public void TopicBind(string topic, string exchangeName, string queueName = null)
        {
            var topicConsumer = cmessagingConsumer as TopicConsumer;
            if (topicConsumer != null)
            {
                topicConsumer.TopicBind(topic, exchangeName, queueName);
            }
            hermesConsumer.TopicBind(topic, exchangeName, queueName);
        }

        #endregion

        public BlockingQueue<ConsumerCallbackEventArgs> BlockingQ
        {
            get { return blockingQ; }
            set 
            {
                blockingQ = value;
                hermesConsumer.BlockingQ = value;
                cmessagingConsumer.BlockingQ = value;
            }
        }

        #region IMessageConsumer Members

        public event Event.ConsumerCallbackEventHandler Callback
        {
            add
            {
                callback += value;
                cmessagingConsumer.Callback += value;
                hermesConsumer.Callback += value;
            }
            remove
            {
                callback -= value;
                cmessagingConsumer.Callback -= value;
                hermesConsumer.Callback -= value;
            }
        }

        public uint BatchSize
        {
            get
            {
                return cmessagingConsumer.BatchSize;
            }
            set
            {
                hermesConsumer.BatchSize = value;
                cmessagingConsumer.BatchSize = value;
            }
        }

        public uint ReceiveTimeout
        {
            get
            {
                return cmessagingConsumer.ReceiveTimeout;
            }
            set
            {
                hermesConsumer.ReceiveTimeout = value;
                cmessagingConsumer.ReceiveTimeout = value;
            }
        }

        public string Identifier
        {
            get
            {
                return cmessagingConsumer.Identifier;
            }
            set
            {
                hermesConsumer.Identifier = value;
                cmessagingConsumer.Identifier = value;
            }
        }

        public void Consume()
        {
            hermesConsumer.StartBlockConsume();
            cmessagingConsumer.StartBlockConsume();
            if (callback != null) callback(this, blockingQ.Take());
        }

        public IMessage ConsumeOne()
        {
            hermesConsumer.StartBlockConsume();
            cmessagingConsumer.StartBlockConsume();
            return blockingQ.Take().Message;
        }

        public void ConsumeAsync(int maxThread = 1, bool autoAck = true)
        {
            hermesConsumer.ConsumeAsync(maxThread, autoAck);
            cmessagingConsumer.ConsumeAsync(maxThread, autoAck);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            cmessagingConsumer.Dispose();
            hermesConsumer.Dispose();
            isDisposed = true;
        }

        #endregion

        #region IQueueConsumer Members

        public void QueueBind(string exchangeName)
        {
            var queueConsumer = cmessagingConsumer as QueueConsumer;
            if (queueConsumer != null)
            {
                queueConsumer.QueueBind(exchangeName);
            }
            hermesConsumer.QueueBind(exchangeName);
        }

        #endregion

        #region IDisposed Members

        public bool IsDispose { get { return isDisposed; } }

        #endregion
    }
}
