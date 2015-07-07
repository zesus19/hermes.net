using System;
using Freeway.Logging;
using Com.Dianping.Cat;
using Com.Dianping.Cat.Message;
using Arch.CMessaging.Client.Core.Utils;
using System.Collections.Generic;
using Arch.CMessaging.Client.Core.Message;

namespace Arch.CMessaging.Client.Consumer.Api
{
    public abstract class BaseMessageListener : IMessageListener
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(BaseMessageListener));

        private string groupId;

        public BaseMessageListener(String groupId)
        {
            this.groupId = groupId;
        }

        public void OnMessage(List<IConsumerMessage> msgs)
        {
            if (msgs != null && msgs.Count != 0)
            {
                String topic = msgs[0].Topic;

                foreach (IConsumerMessage msg in msgs)
                {
                    ITransaction t = Cat.NewTransaction("Message.Consumed", topic);
                    IMessageTree tree = Cat.GetThreadLocalMessageTree();

                    if (msg is PropertiesHolderAware)
                    {
                        PropertiesHolder holder = ((PropertiesHolderAware)msg).PropertiesHolder;
                        String rootMsgId = holder.GetDurableSysProperty(CatConstants.ROOT_MESSAGE_ID);
                        String parentMsgId = holder.GetDurableSysProperty(CatConstants.CURRENT_MESSAGE_ID);

                        tree.RootMessageId = rootMsgId;
                        tree.ParentMessageId = parentMsgId;
                    }

                    try
                    {
                        t.AddData("topic", topic);
                        t.AddData("key", msg.RefKey);
                        t.AddData("groupId", groupId);
                        t.AddData("appId", Cat.Domain);

                        setOnMessageStartTime(msg);
                        OnMessage(msg);
                        setOnMessageEndTime(msg);
                        // by design, if nacked, no effect
                        msg.ack();

                        String ip = Local.IPV4;
                        Cat.LogEvent("Consumer:" + ip, msg.Topic + ":" + groupId, CatConstants.SUCCESS, "key=" + msg.RefKey);
                        Cat.LogEvent("Message:" + topic, "Consumed:" + ip, CatConstants.SUCCESS, "key=" + msg.RefKey);
                        Cat.LogMetricForCount(msg.Topic);
                        t.Status = MessageStatus.SUCCESS.Equals(msg.Status) ? CatConstants.SUCCESS : "FAILED-WILL-RETRY";
                    }
                    catch (Exception e)
                    {
                        Cat.LogError(e);
                        t.SetStatus(e);
                        log.Error("Exception occurred while calling onMessage.", e);
                    }
                    finally
                    {
                        t.Complete();
                    }
                }

            }
        }

        private void setOnMessageEndTime(IConsumerMessage msg)
        {
            if (msg is BaseConsumerMessageAware)
            {
                BaseConsumerMessage baseMsg = ((BaseConsumerMessageAware)msg).BaseConsumerMessage;
                baseMsg.OnMessageEndTimeMills = new DateTime().CurrentTimeMillis();
            }
        }

        private void setOnMessageStartTime(IConsumerMessage msg)
        {
            if (msg is BaseConsumerMessageAware)
            {
                BaseConsumerMessage baseMsg = ((BaseConsumerMessageAware)msg).BaseConsumerMessage;
                baseMsg.OnMessageStartTimeMills = new DateTime().CurrentTimeMillis();
            }
        }

        protected abstract void OnMessage(IConsumerMessage msg);

        public abstract Type MessageType();
    }
}

