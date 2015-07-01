using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Arch.CMessaging.Client.Core.Future
{
    public abstract class AbstractFuture<T> : IFuture<T>, IDisposable
    {
        private object val;
        private bool disposed;
        private volatile Boolean ready;
        private readonly ManualResetEventSlim readyEvent = new ManualResetEventSlim(false);
        
        #region IFuture<T> Members

        public bool IsCancelled { get; private set; }

        public bool IsDone
        {
            get { return ready; }
        }

        public T Get()
        {
            return Get(Timeout.Infinite);
        }

        public T Get(int timeoutInMills)
        {
            T result = default(T);
            if (!Await(timeoutInMills)) throw new TimeoutException();
            else
            {
                if (Value is Exception) throw Value as Exception;
                else result = (T)Value;
            }
            return result;
        }

        public virtual bool Cancel(bool mayInterruptIfRunning)
        {
            IsCancelled = true;
            Value = new OperationCanceledException();
            return IsCancelled;
        }

        #endregion

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(Boolean disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    ((IDisposable)readyEvent).Dispose();
                    disposed = true;
                }
            }
        }

        protected object Value 
        {
            get { return val; }
            set
            {
                lock (this)
                {
                    if (ready) return;
                    ready = true;
                    val = value;
                    readyEvent.Set();
                }
            }
        }

        private bool Await(int timeoutInMills)
        {
            if (ready) return ready;

            readyEvent.Wait(timeoutInMills);
            if (ready) readyEvent.Dispose();

            return ready;
        }
    }
}
