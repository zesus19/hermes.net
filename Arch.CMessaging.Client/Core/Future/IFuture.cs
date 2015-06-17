using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arch.CMessaging.Client.Core.Future
{
    public interface IFuture<T>
    {
        bool IsCancelled { get; }
        bool IsDone { get; }
        T Get();
        T Get(int timeoutInMills);
        bool Cancel(bool mayInterruptIfRunning);
    }
}
