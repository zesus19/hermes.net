using System;
using Arch.CMessaging.Client.Net.Core.Filterchain;
using Arch.CMessaging.Client.Net.Core.Future;
using Arch.CMessaging.Client.Net.Core.Session;
using Arch.CMessaging.Client.Net.Core.Write;

namespace Arch.CMessaging.Client.Net.Filter.Executor
{
    public class WriteRequestFilter : IoFilterAdapter
    {
        private readonly IoEventQueueHandler _queueHandler;

        public WriteRequestFilter()
            : this(new IoEventQueueThrottle())
        { }

        public WriteRequestFilter(IoEventQueueHandler queueHandler)
        {
            if (queueHandler == null)
                throw new ArgumentNullException("queueHandler");
            _queueHandler = queueHandler;
        }

        public IoEventQueueHandler QueueHandler
        {
            get { return _queueHandler; }
        }

        
        public override void FilterWrite(INextFilter nextFilter, IoSession session, IWriteRequest writeRequest)
        {
            IoEvent ioe = new IoEvent(IoEventType.Write, session, writeRequest);
            if (_queueHandler.Accept(this, ioe))
            {
                nextFilter.FilterWrite(session, writeRequest);
                IWriteFuture writeFuture = writeRequest.Future;
                if (writeFuture == null)
                    return;

                // We can track the write request only when it has a future.
                _queueHandler.Offered(this, ioe);
                writeFuture.Complete += (s, e) => _queueHandler.Polled(this, ioe);
            }
        }
    }
}
