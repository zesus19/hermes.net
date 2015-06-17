using System;
using Arch.CMessaging.Client.Net.Core.Session;

namespace Arch.CMessaging.Client.Net.Filter.Executor
{
    public class UnorderedThreadPoolExecutor : ThreadPoolExecutor, IoEventExecutor
    {
        private readonly IoEventQueueHandler _queueHandler;

        public UnorderedThreadPoolExecutor()
            : this(null)
        { }

        public UnorderedThreadPoolExecutor(IoEventQueueHandler queueHandler)
        {
            _queueHandler = queueHandler == null ? NoopIoEventQueueHandler.Instance : queueHandler;
        }

        public IoEventQueueHandler QueueHandler
        {
            get { return _queueHandler; }
        }

        
        public void Execute(IoEvent ioe)
        {
            Boolean offeredEvent = _queueHandler.Accept(this, ioe);
            if (offeredEvent)
            {
                Execute(() =>
                {
                    _queueHandler.Polled(this, ioe);
                    ioe.Fire();
                });

                _queueHandler.Offered(this, ioe);
            }
        }
    }
}
