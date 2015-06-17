using System;

namespace Arch.CMessaging.Client.Net.Core.Future
{
    public interface IReadFuture : IoFuture
    {
        Object Message { get; set; }
        Boolean Read { get; }
        Boolean Closed { get; set; }
        Exception Exception { get; set; }
        new IReadFuture Await();
    }
}
