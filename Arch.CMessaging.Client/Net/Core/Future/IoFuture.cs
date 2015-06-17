using System;
using Arch.CMessaging.Client.Net.Core.Session;

namespace Arch.CMessaging.Client.Net.Core.Future
{
    public interface IoFuture
    {
        IoSession Session { get; }
        Boolean Done { get; }
        event EventHandler<IoFutureEventArgs> Complete;
        IoFuture Await();
        Boolean Await(Int32 millisecondsTimeout);
    }

    public class IoFutureEventArgs : EventArgs
    {
        private readonly IoFuture _future;
        public IoFutureEventArgs(IoFuture future)
        {
            _future = future;
        }
        public IoFuture Future
        {
            get { return _future; }
        }
    }
}
