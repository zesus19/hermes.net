using System;
using Arch.CMessaging.Client.Net.Core.Session;

namespace Arch.CMessaging.Client.Net.Filter.Executor
{
    public interface IoEventQueueHandler
    {
        Boolean Accept(Object source, IoEvent ioe);
        void Offered(Object source, IoEvent ioe);
        void Polled(Object source, IoEvent ioe);
    }

    class NoopIoEventQueueHandler : IoEventQueueHandler
    {
        public static readonly NoopIoEventQueueHandler Instance = new NoopIoEventQueueHandler();

        private NoopIoEventQueueHandler()
        { }

        public Boolean Accept(Object source, IoEvent ioe)
        {
            return true;
        }

        public void Offered(Object source, IoEvent ioe)
        {
            // NOOP
        }

        public void Polled(Object source, IoEvent ioe)
        {
            // NOOP
        }
    }
}
