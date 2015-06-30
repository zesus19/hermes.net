using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arch.CMessaging.Client.Core.Future
{
    public class SettableFuture<T> : AbstractFuture<T>
    {
        public static SettableFuture<T> Create()
        {
            return new SettableFuture<T>();
        }
    }
}
