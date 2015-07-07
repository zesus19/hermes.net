using System;
using System.Collections.Generic;
using Arch.CMessaging.Client.Core.Message;

namespace Arch.CMessaging.Client.Consumer
{
    public interface IMessageListener
    {
        void OnMessage(List<IConsumerMessage> messages);

        Type MessageType();
    }

}

