using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arch.CMessaging.Client.Core.Future
{
    public class SettableFuture<T> : IFuture<T>
    {
        public static SettableFuture<T> Create()
        {
            throw new NotImplementedException();
        }

        #region IFuture<T> Members

        public bool IsCancelled
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsDone
        {
            get { throw new NotImplementedException(); }
        }

        public T Get()
        {
            throw new NotImplementedException();
        }

        public T Get(int timeoutInMills)
        {
            throw new NotImplementedException();
        }

        public bool Cancel(bool mayInterruptIfRunning)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
