using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arch.CMessaging.Client.Core.Collections;

namespace Arch.CMessaging.Client.Core.Future
{
    public class Futures<T>
    {
        public static void AddCallback(
            IListenableFuture<T> future, 
            IFutureCallback<T> callback, ProducerConsumer<FutureCallbackItem<T>> executor)
        {
            future.AddListener(callback, executor);
        }
    }
}
 