using System;
using System.Diagnostics;
using Arch.CMessaging.Client.Net.Core.Session;

namespace Arch.CMessaging.Client.Net.Filter.KeepAlive
{
    public static class KeepAliveRequestTimeoutHandler
    {
        public static IKeepAliveRequestTimeoutHandler Noop
        {
            get { return NoopHandler.Instance; }
        }

        public static IKeepAliveRequestTimeoutHandler Log
        {
            get { return LogHandler.Instance; }
        }

        public static IKeepAliveRequestTimeoutHandler Exception
        {
            get { return ExceptionHandler.Instance; }
        }

        public static IKeepAliveRequestTimeoutHandler Close
        {
            get { return CloseHandler.Instance; }
        }

        public static IKeepAliveRequestTimeoutHandler DeafSpeaker
        {
            get { return DeafSpeakerHandler.Instance; }
        }

        class NoopHandler : IKeepAliveRequestTimeoutHandler
        {
            public static readonly NoopHandler Instance = new NoopHandler();

            private NoopHandler() { }

            public void KeepAliveRequestTimedOut(KeepAliveFilter filter, IoSession session)
            {
                // do nothing
            }
        }

        class LogHandler : IKeepAliveRequestTimeoutHandler
        {
            public static readonly LogHandler Instance = new LogHandler();

            private LogHandler() { }

            public void KeepAliveRequestTimedOut(KeepAliveFilter filter, IoSession session)
            {
                Debug.WriteLine("A keep-alive response message was not received within {0} second(s).", filter.RequestTimeout);
            }
        }

        class ExceptionHandler : IKeepAliveRequestTimeoutHandler
        {
            public static readonly ExceptionHandler Instance = new ExceptionHandler();

            private ExceptionHandler() { }

            public void KeepAliveRequestTimedOut(KeepAliveFilter filter, IoSession session)
            {
                throw new KeepAliveRequestTimeoutException("A keep-alive response message was not received within "
                   + filter.RequestTimeout + " second(s).");
            }
        }

        class CloseHandler : IKeepAliveRequestTimeoutHandler
        {
            public static readonly CloseHandler Instance = new CloseHandler();

            private CloseHandler() { }

            public void KeepAliveRequestTimedOut(KeepAliveFilter filter, IoSession session)
            {
                Debug.WriteLine("Closing the session because a keep-alive response message was not received within {0} second(s).",
                    filter.RequestTimeout);
                session.Close(true);
            }
        }

        class DeafSpeakerHandler : IKeepAliveRequestTimeoutHandler
        {
            public static readonly DeafSpeakerHandler Instance = new DeafSpeakerHandler();

            private DeafSpeakerHandler() { }

            public void KeepAliveRequestTimedOut(KeepAliveFilter filter, IoSession session)
            {
                throw new ApplicationException("Shouldn't be invoked.  Please file a bug report.");
            }
        }
    }
}
