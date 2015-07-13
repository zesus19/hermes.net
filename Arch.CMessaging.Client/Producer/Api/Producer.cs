using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arch.CMessaging.Client.Core.Future;
using Arch.CMessaging.Client.Core.Result;
using Arch.CMessaging.Client.Core.Utils;
using Arch.CMessaging.Client.Producer.Build;

namespace Arch.CMessaging.Client.Producer
{
    public interface IMessageHolder
    {
        IMessageHolder WithPriority();
        IMessageHolder WithRefKey(string key);
        IFuture<SendResult> Send();
        SendResult SendSync();
        IMessageHolder AddProperty(string key, string value);
        IMessageHolder SetCallback(ICompletionCallback<SendResult> callback);
    }

    public abstract class Producer
    {
        public static Producer GetInstance() 
        {
            ComponentsConfigurator.DefineComponents();
			return ComponentLocator.Lookup<Producer>();
        }
        public abstract IMessageHolder Message(string topic, string partitionKey, object body);
    }
}
