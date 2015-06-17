using System;

namespace Arch.CMessaging.Client.Net.Core.Future
{
    public interface IWriteFuture : IoFuture
    {
        Boolean Written { get; set; }
        Exception Exception { get; set; }
        new IWriteFuture Await();
    }
}
