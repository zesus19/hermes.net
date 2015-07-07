using System;
using Arch.CMessaging.Client.Core.Ioc;
using Arch.CMessaging.Client.Core.Utils;

namespace Arch.CMessaging.Client.Consumer
{
    public abstract class Consumer
    {
        public static Consumer GetInstance()
        {
            return ComponentLocator.Lookup<Consumer>();
        }

        public abstract IConsumerHolder Start(String topic, String groupId, IMessageListener listener);
    }

    public interface IConsumerHolder
    {
        void close();
    }
}

