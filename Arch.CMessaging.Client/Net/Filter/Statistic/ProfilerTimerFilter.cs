using System;
using System.Threading;
using Arch.CMessaging.Client.Net.Core.Filterchain;
using Arch.CMessaging.Client.Net.Core.Session;
using Arch.CMessaging.Client.Net.Core.Write;

namespace Arch.CMessaging.Client.Net.Filter.Statistic
{
    public class ProfilerTimerFilter : IoFilterAdapter
    {
        private volatile TimeUnit _timeUnit;

        private Boolean _profileMessageReceived;
        private TimerWorker _messageReceivedTimerWorker;
        private Boolean _profileMessageSent;
        private TimerWorker _messageSentTimerWorker;
        private Boolean _profileSessionCreated;
        private TimerWorker _sessionCreatedTimerWorker;
        private Boolean _profileSessionOpened;
        private TimerWorker _sessionOpenedTimerWorker;
        private Boolean _profileSessionIdle;
        private TimerWorker _sessionIdleTimerWorker;
        private Boolean _profileSessionClosed;
        private TimerWorker _sessionClosedTimerWorker;

        public ProfilerTimerFilter()
            : this(TimeUnit.Milliseconds, IoEventType.MessageReceived | IoEventType.MessageSent)
        { }

        public ProfilerTimerFilter(TimeUnit timeUnit)
            : this(timeUnit, IoEventType.MessageReceived | IoEventType.MessageSent)
        { }

        public ProfilerTimerFilter(TimeUnit timeUnit, IoEventType eventTypes)
        {
            _timeUnit = timeUnit;
            SetProfilers(eventTypes);
        }

        public TimeUnit TimeUnit
        {
            get { return _timeUnit; }
            set { _timeUnit = value; }
        }

        public IoEventType EventsToProfile
        {
            get
            {
                IoEventType et = default(IoEventType);
                
                if (_profileMessageReceived)
                    et |= IoEventType.MessageReceived;
                if (_profileMessageSent)
                    et |= IoEventType.MessageSent;
                if (_profileSessionCreated)
                    et |= IoEventType.SessionCreated;
                if (_profileSessionOpened)
                    et |= IoEventType.SessionOpened;
                if (_profileSessionIdle)
                    et |= IoEventType.SessionIdle;
                if (_profileSessionClosed)
                    et |= IoEventType.SessionClosed;

                return et;
            }
            set { SetProfilers(value); }
        }

        public Double GetAverageTime(IoEventType type)
        {
            return GetTimerWorker(type).Average;
        }

        public Int64 GetTotalCalls(IoEventType type)
        {
            return GetTimerWorker(type).callsNumber;
        }

        public Int64 GetTotalTime(IoEventType type)
        {
            return GetTimerWorker(type).total;
        }

        public Int64 GetMinimumTime(IoEventType type)
        {
            return GetTimerWorker(type).minimum;
        }

        public Int64 GetMaximumTime(IoEventType type)
        {
            return GetTimerWorker(type).maximum;
        }

        public override void MessageReceived(INextFilter nextFilter, IoSession session, Object message)
        {
            Profile(_profileMessageReceived, _messageReceivedTimerWorker, () => nextFilter.MessageReceived(session, message));
        }
    
        public override void MessageSent(INextFilter nextFilter, IoSession session, IWriteRequest writeRequest)
        {
            Profile(_profileMessageSent, _messageSentTimerWorker, () => nextFilter.MessageSent(session, writeRequest));
        }
        
        public override void SessionCreated(INextFilter nextFilter, IoSession session)
        {
            Profile(_profileSessionCreated, _sessionCreatedTimerWorker, () => nextFilter.SessionCreated(session));
        }
       
        public override void SessionOpened(INextFilter nextFilter, IoSession session)
        {
            Profile(_profileSessionOpened, _sessionOpenedTimerWorker, () => nextFilter.SessionOpened(session));
        }
        
        public override void SessionIdle(INextFilter nextFilter, IoSession session, IdleStatus status)
        {
            Profile(_profileSessionIdle, _sessionIdleTimerWorker, () => nextFilter.SessionIdle(session, status));
        }
       
        public override void SessionClosed(INextFilter nextFilter, IoSession session)
        {
            Profile(_profileSessionClosed, _sessionClosedTimerWorker, () => nextFilter.SessionClosed(session));
        }

        private void Profile(Boolean profile, TimerWorker worker, Action action)
        {
            if (profile)
            {
                Int64 start = TimeNow();
                action();
                Int64 end = TimeNow();
                worker.AddNewDuration(end - start);
            }
            else
            {
                action();
            }
        }

        private TimerWorker GetTimerWorker(IoEventType type)
        {
            switch (type)
            {
                case IoEventType.MessageReceived:
                    if (_profileMessageReceived)
                        return _messageReceivedTimerWorker;
                    break;
                case IoEventType.MessageSent:
                    if (_profileMessageSent)
                        return _messageSentTimerWorker;
                    break;
                case IoEventType.SessionCreated:
                    if (_profileSessionCreated)
                        return _sessionCreatedTimerWorker;
                    break;
                case IoEventType.SessionOpened:
                    if (_profileSessionOpened)
                        return _sessionOpenedTimerWorker;
                    break;
                case IoEventType.SessionIdle:
                    if (_profileSessionIdle)
                        return _sessionIdleTimerWorker;
                    break;
                case IoEventType.SessionClosed:
                    if (_profileSessionClosed)
                        return _sessionClosedTimerWorker;
                    break;
                default:
                    break;
            }

            throw new ArgumentException("You are not monitoring this event. Please add this event first.");
        }

        private void SetProfilers(IoEventType eventTypes)
        {
            if ((eventTypes & IoEventType.MessageReceived) == IoEventType.MessageReceived)
            {
                _messageReceivedTimerWorker = new TimerWorker();
                _profileMessageReceived = true;
            }
            if ((eventTypes & IoEventType.MessageSent) == IoEventType.MessageSent)
            {
                _messageSentTimerWorker = new TimerWorker();
                _profileMessageSent = true;
            }
            if ((eventTypes & IoEventType.SessionCreated) == IoEventType.SessionCreated)
            {
                _sessionCreatedTimerWorker = new TimerWorker();
                _profileSessionCreated = true;
            }
            if ((eventTypes & IoEventType.SessionOpened) == IoEventType.SessionOpened)
            {
                _sessionOpenedTimerWorker = new TimerWorker();
                _profileSessionOpened = true;
            }
            if ((eventTypes & IoEventType.SessionIdle) == IoEventType.SessionIdle)
            {
                _sessionIdleTimerWorker = new TimerWorker();
                _profileSessionIdle = true;
            }
            if ((eventTypes & IoEventType.SessionClosed) == IoEventType.SessionClosed)
            {
                _sessionClosedTimerWorker = new TimerWorker();
                _profileSessionClosed = true;
            }
        }

        private Int64 TimeNow()
        {
            switch (_timeUnit)
            {
                case TimeUnit.Seconds:
                    return DateTime.Now.Ticks / TimeSpan.TicksPerSecond;
                case TimeUnit.Ticks:
                    return DateTime.Now.Ticks;
                default:
                    return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            }
        }

        class TimerWorker
        {
            public Int64 total;
            public Int64 callsNumber;
            public Int64 minimum = Int64.MaxValue;
            public Int64 maximum;
            private Object _syncRoot = new Byte[0];

            public void AddNewDuration(Int64 duration)
            {
                Interlocked.Increment(ref callsNumber);
                Interlocked.Add(ref total, duration);
                lock (_syncRoot)
                {
                    if (duration < minimum)
                        minimum = duration;
                    if (duration > maximum)
                        maximum = duration;
                }
            }

            public Double Average
            {
                get { return total / callsNumber; }
            }
        }
    }

    public enum TimeUnit
    {
        Seconds,
        Milliseconds,
        Ticks
    }
}
