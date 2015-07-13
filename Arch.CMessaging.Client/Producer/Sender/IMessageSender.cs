using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arch.CMessaging.Client.Core.Future;
using Arch.CMessaging.Client.Core.Message;
using Arch.CMessaging.Client.Core.Result;

namespace Arch.CMessaging.Client.Producer.Sender
{
    public interface IMessageSender
    {
        IFuture<SendResult> Send(ProducerMessage message);

        void Resend(ProducerMessage msg, SettableFuture<SendResult> future);
    }
}
