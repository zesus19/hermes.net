using System;
using Arch.CMessaging.Client.Core.Ioc;
using Arch.CMessaging.Client.Core.Utils;
using Arch.CMessaging.Client.Producer.Build;

namespace Arch.CMessaging.Client.Consumer
{
    public abstract class Consumer
    {
        public static Consumer GetInstance()
        {
            ComponentsConfigurator.DefineComponents();
            return ComponentLocator.Lookup<Consumer>();
        }

        public abstract IConsumerHolder Start(String topic, String groupId, IMessageListener listener);
    }

    public interface IConsumerHolder
    {
        void close();
    }
}

