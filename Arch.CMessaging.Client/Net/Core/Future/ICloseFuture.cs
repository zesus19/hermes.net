using System;

namespace Arch.CMessaging.Client.Net.Core.Future
{

    public interface ICloseFuture : IoFuture
    {
        Boolean Closed { get; set; }
        new ICloseFuture Await();
    }
}
