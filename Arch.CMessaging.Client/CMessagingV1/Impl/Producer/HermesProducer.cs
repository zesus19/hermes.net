using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Arch.CMessaging.Client.API;
using Arch.CMessaging.Client.Newtonsoft.Json;
using Arch.CMessaging.Client.Producer;
using Arch.CMessaging.Client.Core.Utils;

namespace Arch.CMessaging.Client.Impl.Producer
{
    public class HermesProducer : IMessageProducer
    {
        private string exchangeName;
        #region IMessageProducer Members

        public event Event.BrokerAckEventHandler BrokerAcks;

        public event Event.BrokerNackEventHandler BrokerNacks;

        public event Event.CallbackExceptionEventHandler CallbackException;

        public event Event.FlowControlEventHandler FlowControl;

        public uint WindowSize { get; set; }

        public bool UseFlowControl { get; set; }

        public bool IsUnderFlowControl { get { return false; } }

        public string Identifier { get; set; }

        public void ExchangeDeclare(string exchangeName)
        {
            Arch.CMessaging.Core.Util.Guard.ArgumentNotNullOrEmpty(exchangeName, "exchangeName");
            Arch.CMessaging.Core.Util.Guard.ArgumentNotNullOrEmpty(Identifier, "Identifier");
            this.exchangeName = exchangeName.Trim();
        }

        public void PublishAsync<TMessage>(TMessage message, string subject, CMessaging.Core.Content.MessageHeader header = null)
        {
            var producer = Arch.CMessaging.Client.Producer.Producer.GetInstance();
            var holder = producer.Message(
                string.Format("{0}{1}", exchangeName, !string.IsNullOrEmpty(subject) ? "." + subject : string.Empty), System.Net.Dns.GetHostName(), message);
            holder.AddProperty("appid", ConfigurationManager.AppSettings["AppID"]);
            holder.AddProperty("messageid", Guid.NewGuid().ToString());
            holder.AddProperty("exchangename", exchangeName);
            holder.AddProperty("rawtype", typeof(TMessage).Name);
            holder.AddProperty("subject", string.IsNullOrEmpty(subject) ? string.Empty : subject);
            holder.AddProperty("timestamp", DateTime.Now.CurrentTimeSeconds().ToString());
            if (header != null && header.UserHeader != null)
            {
                holder.AddProperty("cmessage_userhead_#", JsonConvert.SerializeObject(header.UserHeader));
            }
            holder.SendSync();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            
        }

        #endregion
    }
}
