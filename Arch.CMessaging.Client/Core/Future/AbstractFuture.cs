using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arch.CMessaging.Client.Core.Future
{
    public abstract class AbstractFuture<T> : IFuture<T>
    {
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

        public virtual bool Cancel(bool mayInterruptIfRunning)
        {
            throw new NotImplementedException();
        }

        #endregion

        public bool Set(T val)
        {
            return false;
        }

        public bool SetException(Exception ex)
        {
            return false;
        }
    }
}
