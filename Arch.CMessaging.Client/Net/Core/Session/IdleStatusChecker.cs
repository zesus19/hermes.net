using System;
using System.Collections.Generic;
using System.Threading;

namespace Arch.CMessaging.Client.Net.Core.Session
{
    public class IdleStatusChecker : IDisposable
    {
        public const Int32 IdleCheckingInterval = 1000;

        private readonly Timer _idleTimer;
        private Int32 _interval;

        public IdleStatusChecker(Func<IEnumerable<IoSession>> getSessionsFunc)
            : this(IdleCheckingInterval, getSessionsFunc)
        { }

        public IdleStatusChecker(Int32 interval, Func<IEnumerable<IoSession>> getSessionsFunc)
        {
            _interval = interval;
            _idleTimer = new Timer(o =>
            {
                AbstractIoSession.NotifyIdleness(getSessionsFunc(), DateTime.Now);
            });
        }

        public Int32 Interval
        {
            get { return _interval; }
            set { _interval = value; }
        }

        public void Start()
        {
            _idleTimer.Change(0, _interval);
        }

        public void Stop()
        {
            _idleTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                _idleTimer.Dispose();
            }
        }
    }
}
