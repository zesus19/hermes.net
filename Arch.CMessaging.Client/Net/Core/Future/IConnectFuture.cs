using System;
using Arch.CMessaging.Client.Net.Core.Session;

namespace Arch.CMessaging.Client.Net.Core.Future
{
    public interface IConnectFuture : IoFuture
    {
        Boolean Connected { get; }
        Boolean Canceled { get; }
        Exception Exception { get; set; }
        void SetSession(IoSession session);
        void Cancel();
        new IConnectFuture Await();
    }
}
