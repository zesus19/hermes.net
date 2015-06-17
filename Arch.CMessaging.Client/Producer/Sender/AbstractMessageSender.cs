using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arch.CMessaging.Client.Core.Message.Partition;
using Arch.CMessaging.Client.Core.Meta;
using Arch.CMessaging.Client.Producer.Monitor;
using Arch.CMessaging.Client.Transport.EndPoint;
using Arch.CMessaging.Client.Core.Result;
using Arch.CMessaging.Client.Core.Future;
using Arch.CMessaging.Client.Core.Message;

namespace Arch.CMessaging.Client.Producer.Sender
{
    public abstract class AbstractMessageSender : IMessageSender
    {
        //ioc inject
        protected IEndpointManager endpointManager;

        //ioc inject
        protected IEndpointClient endpointClient;

        //ioc inject
        protected IPartitioningStrategy partitioningAlgo;

        //ioc inject
        protected IMetaService metaService;

        //ioc inject
        protected ISendMessageAcceptanceMonitor messageAcceptanceMonitor;

        //ioc inject
        protected ISendMessageResultMonitor messageResultMonitor;

        #region IMessageSender Members

        public IFuture<SendResult> Send(ProducerMessage message)
        {
            PreSend(message);
            return DoSend(message);
        }

        #endregion

        protected abstract IFuture<SendResult> DoSend(ProducerMessage message);
        protected void PreSend(ProducerMessage message) 
        {
            var partitionNo = partitioningAlgo.ComputePartitionNo(
                message.PartitionKey, 
                metaService.ListPartitionsByTopic(message.Topic).Count);
            message.Partition = partitionNo;
	    }
    }
}
