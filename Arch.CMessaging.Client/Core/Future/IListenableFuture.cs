using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arch.CMessaging.Client.Core.Collections;

namespace Arch.CMessaging.Client.Core.Future
{
    public interface IListenableFuture<T> : IFuture<T>
    {
        void AddListener(IFutureCallback<T> callback, ProducerConsumer<FutureCallbackItem<T>> executor);
    }
}
