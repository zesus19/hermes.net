using System;
using Arch.CMessaging.Client.Net.Core.Session;
using Arch.CMessaging.Client.Net.Core.Write;
using System.Diagnostics;

namespace Arch.CMessaging.Client.Net.Core.Filterchain
{
    public class IoFilterEvent : IoEvent
    {
        private readonly INextFilter _nextFilter;

        public IoFilterEvent(INextFilter nextFilter, IoEventType eventType, IoSession session, Object parameter)
            : base(eventType, session, parameter)
        {
            if (nextFilter == null)
                throw new ArgumentNullException("nextFilter");
            _nextFilter = nextFilter;
        }

        public INextFilter NextFilter
        {
            get { return _nextFilter; }
        }

        
        public override void Fire()
        {
            Debug.WriteLine("Firing a {0} event for session {1}", EventType, Session.Id);

            switch (EventType)
            {
                case IoEventType.MessageReceived:
                    _nextFilter.MessageReceived(Session, Parameter);
                    break;
                case IoEventType.MessageSent:
                    _nextFilter.MessageSent(Session, (IWriteRequest)Parameter);
                    break;
                case IoEventType.Write:
                    _nextFilter.FilterWrite(Session, (IWriteRequest)Parameter);
                    break;
                case IoEventType.Close:
                    _nextFilter.FilterClose(Session);
                    break;
                case IoEventType.ExceptionCaught:
                    _nextFilter.ExceptionCaught(Session, (Exception)Parameter);
                    break;
                case IoEventType.SessionIdle:
                    _nextFilter.SessionIdle(Session, (IdleStatus)Parameter);
                    break;
                case IoEventType.SessionCreated:
                    _nextFilter.SessionCreated(Session);
                    break;
                case IoEventType.SessionOpened:
                    _nextFilter.SessionOpened(Session);
                    break;
                case IoEventType.SessionClosed:
                    _nextFilter.SessionClosed(Session);
                    break;
                default:
                    throw new InvalidOperationException("Unknown event type: " + EventType);
            }

            Debug.WriteLine("Event {0} has been fired for session {1}", EventType, Session.Id);
        }
    }
}
