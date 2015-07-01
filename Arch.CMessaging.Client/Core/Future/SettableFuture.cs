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

        public void Set(T val)
        {
            base.Value = val;
        }

        public void SetException(Exception ex)
        {
            base.Value = ex;
        }
    }
}
