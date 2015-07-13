using System;
using Arch.CMessaging.Client.Net.Core.Session;
using System.Collections.Generic;
using Arch.CMessaging.Client.Transport.Command;
using Arch.CMessaging.Client.MetaEntity.Entity;
using Arch.CMessaging.Client.Core.Bo;

namespace Arch.CMessaging.Client.Core.Message
{
    public class BrokerConsumerMessage : IConsumerMessage, PropertiesHolderAware, BaseConsumerMessageAware
    {
        public BaseConsumerMessage BaseConsumerMessage{ get; private set; }

        public long MsgSeq{ get; set; }

        public int Partition{ get; set; }

        public bool Priority{ get; set; }

        public bool Resend { get; set; }

        public string GroupId{ get; set; }

        public long CorrelationId{ get; set; }

        public IoSession Channel{ get; set; }

        public BrokerConsumerMessage(BaseConsumerMessage baseMsg)
        {
            this.BaseConsumerMessage = baseMsg;
        }

        public void nack()
        {
            if (BaseConsumerMessage.nack())
            {
                AckMessageCommand cmd = new AckMessageCommand();
                cmd.Header.CorrelationId = CorrelationId;
                Tpp tpp = new Tpp(BaseConsumerMessage.Topic, Partition, Priority);
                cmd.addNackMsg(tpp, GroupId, Resend, MsgSeq, BaseConsumerMessage.RemainingRetries, BaseConsumerMessage.OnMessageStartTimeMills, BaseConsumerMessage.OnMessageEndTimeMills);
                Channel.Write(cmd);
            }
        }

        
        public string GetProperty(string name)
        {
            return BaseConsumerMessage.getDurableAppProperty(name);
        }

        
        public IEnumerator<string> GetPropertyNames()
        {
            return BaseConsumerMessage.RawDurableAppPropertyNames;
        }

        
        public long BornTime
        {
            get { return BaseConsumerMessage.BornTime; }
        }

        
        public string Topic
        {
            get{ return BaseConsumerMessage.Topic; }
        }

        
        public string RefKey
        {
            get { return BaseConsumerMessage.RefKey; }
        }

        
        public T GetBody<T>()
        {
            return (T)BaseConsumerMessage.Body;
        }

        
        public void ack()
        {
            if (BaseConsumerMessage.ack())
            {
                AckMessageCommand cmd = new AckMessageCommand();
                cmd.Header.CorrelationId = CorrelationId;
                Tpp tpp = new Tpp(BaseConsumerMessage.Topic, Partition, Priority);
                cmd.addAckMsg(tpp, GroupId, Resend, MsgSeq, BaseConsumerMessage.RemainingRetries, BaseConsumerMessage.OnMessageStartTimeMills, BaseConsumerMessage.OnMessageEndTimeMills);
                Channel.Write(cmd);
            }
        }

        
        public string Status
        {
            get{ return BaseConsumerMessage.Status; }
        }

        public int RemainingRetries
        {
            get{ return BaseConsumerMessage.RemainingRetries; }
        }

        public PropertiesHolder PropertiesHolder
        {
            get{ return BaseConsumerMessage.PropertiesHolder; }
        }

        
        public override string ToString()
        {
            return "BrokerConsumerMessage{" + "m_baseMsg=" + BaseConsumerMessage + ", m_msgSeq=" + MsgSeq + ", m_partition="
            + Partition + ", m_priority=" + Priority + ", m_resend=" + Resend + ", m_groupId='" + GroupId
            + '\'' + ", m_correlationId=" + CorrelationId + ", m_channel=" + Channel + '}';
        }
    }
}

