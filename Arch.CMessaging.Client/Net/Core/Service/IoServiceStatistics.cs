using System;
using System.Threading;

namespace Arch.CMessaging.Client.Net.Core.Service
{
    public class IoServiceStatistics
    {
        private readonly IoService _service;
        private readonly Object _throughputCalculationLock = new Byte[0];

        private Double _readBytesThroughput;
        private Double _writtenBytesThroughput;
        private Double _readMessagesThroughput;
        private Double _writtenMessagesThroughput;
        private Double _largestReadBytesThroughput;
        private Double _largestWrittenBytesThroughput;
        private Double _largestReadMessagesThroughput;
        private Double _largestWrittenMessagesThroughput;
        private Int64 _readBytes;
        private Int64 _writtenBytes;
        private Int64 _readMessages;
        private Int64 _writtenMessages;
        private DateTime _lastReadTime;
        private DateTime _lastWriteTime;
        private DateTime _lastThroughputCalculationTime;
        private Int64 _lastReadBytes;
        private Int64 _lastWrittenBytes;
        private Int64 _lastReadMessages;
        private Int64 _lastWrittenMessages;
        private Int32 _scheduledWriteBytes;
        private Int32 _scheduledWriteMessages;
        private Int32 _throughputCalculationInterval = 3;

        public IoServiceStatistics(IoService service)
        {
            _service = service;
        }

        public DateTime LastIoTime
        {
            get { return _lastReadTime > _lastWriteTime ? _lastReadTime : _lastWriteTime; }
        }

        public DateTime LastReadTime
        {
            get { return _lastReadTime; }
            set { _lastReadTime = value; }
        }

        public DateTime LastWriteTime
        {
            get { return _lastWriteTime; }
            set { _lastWriteTime = value; }
        }

        public Int64 ReadBytes
        {
            get { return _readBytes; }
        }

        public Int64 WrittenBytes
        {
            get { return _writtenBytes; }
        }

        public Int64 ReadMessages
        {
            get { return _readMessages; }
        }

        public Int64 WrittenMessages
        {
            get { return _writtenMessages; }
        }

        public Double ReadBytesThroughput
        {
            get
            {
                ResetThroughput();
                return _readBytesThroughput;
            }
        }

        public Double WrittenBytesThroughput
        {
            get
            {
                ResetThroughput();
                return _writtenBytesThroughput;
            }
        }

        public Double ReadMessagesThroughput
        {
            get
            {
                ResetThroughput();
                return _readMessagesThroughput;
            }
        }

        public Double WrittenMessagesThroughput
        {
            get
            {
                ResetThroughput();
                return _writtenMessagesThroughput;
            }
        }

        public Double LargestReadBytesThroughput
        {
            get { return _largestReadBytesThroughput; }
        }

        public Double LargestWrittenBytesThroughput
        {
            get { return _largestWrittenBytesThroughput; }
        }

        public Double LargestReadMessagesThroughput
        {
            get { return _largestReadMessagesThroughput; }
        }

        public Double LargestWrittenMessagesThroughput
        {
            get { return _largestWrittenMessagesThroughput; }
        }

        public Int32 ThroughputCalculationInterval
        {
            get { return _throughputCalculationInterval; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("ThroughputCalculationInterval should be greater than 0", "value");
                _throughputCalculationInterval = value;
            }
        }

        public Int64 ThroughputCalculationIntervalInMillis
        {
            get { return _throughputCalculationInterval * 1000L; }
        }

        internal DateTime LastThroughputCalculationTime
        {
            get { return _lastThroughputCalculationTime; }
            set { _lastThroughputCalculationTime = value; }
        }

        public Int32 ScheduledWriteBytes
        {
            get { return _scheduledWriteBytes; }
        }

        public Int32 ScheduledWriteMessages
        {
            get { return _scheduledWriteMessages; }
        }

        public void UpdateThroughput(DateTime currentTime)
        {
            lock (_throughputCalculationLock)
            {
                Int64 interval = (Int64)(currentTime - _lastThroughputCalculationTime).TotalMilliseconds;
                Int64 minInterval = ThroughputCalculationIntervalInMillis;
                if (minInterval == 0 || interval < minInterval)
                {
                    return;
                }

                Int64 readBytes = _readBytes;
                Int64 writtenBytes = _writtenBytes;
                Int64 readMessages = _readMessages;
                Int64 writtenMessages = _writtenMessages;

                _readBytesThroughput = (readBytes - _lastReadBytes) * 1000.0 / interval;
                _writtenBytesThroughput = (writtenBytes - _lastWrittenBytes) * 1000.0 / interval;
                _readMessagesThroughput = (readMessages - _lastReadMessages) * 1000.0 / interval;
                _writtenMessagesThroughput = (writtenMessages - _lastWrittenMessages) * 1000.0 / interval;

                if (_readBytesThroughput > _largestReadBytesThroughput)
                {
                    _largestReadBytesThroughput = _readBytesThroughput;
                }
                if (_writtenBytesThroughput > _largestWrittenBytesThroughput)
                {
                    _largestWrittenBytesThroughput = _writtenBytesThroughput;
                }
                if (_readMessagesThroughput > _largestReadMessagesThroughput)
                {
                    _largestReadMessagesThroughput = _readMessagesThroughput;
                }
                if (_writtenMessagesThroughput > _largestWrittenMessagesThroughput)
                {
                    _largestWrittenMessagesThroughput = _writtenMessagesThroughput;
                }

                _lastReadBytes = readBytes;
                _lastWrittenBytes = writtenBytes;
                _lastReadMessages = readMessages;
                _lastWrittenMessages = writtenMessages;

                _lastThroughputCalculationTime = currentTime;
            }
        }

        public void IncreaseReadBytes(Int64 increment, DateTime currentTime)
        {
            Interlocked.Add(ref _readBytes, increment);
            _lastReadTime = currentTime;
        }

        public void IncreaseReadMessages(DateTime currentTime)
        {
            Interlocked.Increment(ref _readMessages);
            _lastReadTime = currentTime;
        }

        public void IncreaseWrittenBytes(Int32 increment, DateTime currentTime)
        {
            Interlocked.Add(ref _writtenBytes, increment);
            _lastWriteTime = currentTime;
        }

        public void IncreaseWrittenMessages(DateTime currentTime)
        {
            Interlocked.Increment(ref _writtenMessages);
            _lastWriteTime = currentTime;
        }

        public void IncreaseScheduledWriteBytes(Int32 increment)
        {
            Interlocked.Add(ref _scheduledWriteBytes, increment);
        }

        public void IncreaseScheduledWriteMessages()
        {
            Interlocked.Increment(ref _scheduledWriteMessages);
        }

        public void DecreaseScheduledWriteMessages()
        {
            Interlocked.Decrement(ref _scheduledWriteMessages);
        }

        private void ResetThroughput()
        {
            if (_service.ManagedSessions.Count == 0)
            {
                _readBytesThroughput = 0;
                _writtenBytesThroughput = 0;
                _readMessagesThroughput = 0;
                _writtenMessagesThroughput = 0;
            }
        }
    }
}
