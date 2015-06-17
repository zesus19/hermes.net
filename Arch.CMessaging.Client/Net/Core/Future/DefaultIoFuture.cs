using System;
using System.Threading;
using Arch.CMessaging.Client.Net.Core.Session;
using Arch.CMessaging.Client.Net.Util;

namespace Arch.CMessaging.Client.Net.Core.Future
{

    public class DefaultIoFuture : IoFuture, IDisposable
    {
        private readonly IoSession _session;
        private volatile Boolean _ready;
        private readonly ManualResetEventSlim _readyEvent = new ManualResetEventSlim(false);
        private Object _value;
        private EventHandler<IoFutureEventArgs> _complete;
        private Boolean _disposed;

        public DefaultIoFuture(IoSession session)
        {
            _session = session;
        }

        public event EventHandler<IoFutureEventArgs> Complete
        {
            add
            {
                EventHandler<IoFutureEventArgs> tmp;
                EventHandler<IoFutureEventArgs> complete = _complete;
                do
                {
                    tmp = complete;
                    EventHandler<IoFutureEventArgs> newComplete = (EventHandler<IoFutureEventArgs>)Delegate.Combine(tmp, value);
                    complete = Interlocked.CompareExchange(ref _complete, newComplete, tmp);
                }
                while (complete != tmp);

                if (_ready)
                    OnComplete(value);
            }
            remove
            {
                EventHandler<IoFutureEventArgs> tmp;
                EventHandler<IoFutureEventArgs> complete = _complete;
                do
                {
                    tmp = complete;
                    EventHandler<IoFutureEventArgs> newComplete = (EventHandler<IoFutureEventArgs>)Delegate.Remove(tmp, value);
                    complete = Interlocked.CompareExchange(ref _complete, newComplete, tmp);
                }
                while (complete != tmp);
            }
        }

        public virtual IoSession Session
        {
            get { return _session; }
        }

        public Boolean Done
        {
            get { return _ready; }
        }

        public Object Value
        {
            get { return _value; }
            set
            {
                lock (this)
                {
                    if (_ready)
                        return;
                    _ready = true;
                    _value = value;
                    _readyEvent.Set();
                }
                OnComplete();
            }
        }

        public IoFuture Await()
        {
            Await0(Timeout.Infinite);
            return this;
        }

        public Boolean Await(Int32 millisecondsTimeout)
        {
            return Await0(millisecondsTimeout);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(Boolean disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    ((IDisposable)_readyEvent).Dispose();
                    _disposed = true;
                }
            }
        }

        private Boolean Await0(Int32 millisecondsTimeout)
        {
            if (_ready)
                return _ready;

            _readyEvent.Wait(millisecondsTimeout);
            if (_ready)
                _readyEvent.Dispose();

            return _ready;
        }

        private void OnComplete()
        {
            EventHandler<IoFutureEventArgs> complete = _complete;
            if (complete != null)
            {
                Delegate[] handlers = complete.GetInvocationList();
                foreach (var current in handlers)
                {
                    OnComplete((EventHandler<IoFutureEventArgs>)current);
                }
            }
        }

        private void OnComplete(EventHandler<IoFutureEventArgs> act)
        {
            try
            {
                act(_session, new IoFutureEventArgs(this));
            }
            catch (Exception ex)
            {
                ExceptionMonitor.Instance.ExceptionCaught(ex);
            }
        }
    }
}
