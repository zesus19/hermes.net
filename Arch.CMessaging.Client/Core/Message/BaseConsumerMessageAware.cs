using System;

namespace Arch.CMessaging.Client.Core.Message
{
    public interface BaseConsumerMessageAware
    {
        BaseConsumerMessage BaseConsumerMessage { get; }
    }
}

