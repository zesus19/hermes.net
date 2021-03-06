﻿using System;
using System.Threading;
using Arch.CMessaging.Client.Net.Core.Session;
using System.Diagnostics;

namespace Arch.CMessaging.Client.Net.Filter.Executor
{
    public class IoEventQueueThrottle : IoEventQueueHandler
    {
        private volatile Int32 _threshold;
        private readonly IoEventSizeEstimator _sizeEstimator;
        private readonly Object _syncRoot = new Byte[0];
        private Int32 _counter;
        private Int32 _waiters;

        public IoEventQueueThrottle()
            : this(new DefaultIoEventSizeEstimator(), 65536)
        { }

        public IoEventQueueThrottle(Int32 threshold)
            : this(new DefaultIoEventSizeEstimator(), threshold)
        { }

        public IoEventQueueThrottle(IoEventSizeEstimator sizeEstimator, Int32 threshold)
        {
            if (sizeEstimator == null)
                throw new ArgumentNullException("sizeEstimator");
            _sizeEstimator = sizeEstimator;
            Threshold = threshold;
        }

        public Int32 Threshold
        {
            get { return _threshold; }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Threshold should be greater than 0", "value");
                _threshold = value;
            }
        }

        
        public Boolean Accept(Object source, IoEvent ioe)
        {
            return true;
        }

        public void Offered(Object source, IoEvent ioe)
        {
            Int32 eventSize = EstimateSize(ioe);
            Int32 currentCounter = Interlocked.Add(ref _counter, eventSize);

            Debug.WriteLine(Thread.CurrentThread.Name + " state: " + _counter + " / " + _threshold);

            if (currentCounter >= _threshold)
                Block();
        }

        public void Polled(Object source, IoEvent ioe)
        {
            Int32 eventSize = EstimateSize(ioe);
            Int32 currentCounter = Interlocked.Add(ref _counter, -eventSize);

            Debug.WriteLine(Thread.CurrentThread.Name + " state: " + _counter + " / " + _threshold);

            if (currentCounter < _threshold)
                Unblock();
        }

        protected void Block()
        {
            Debug.WriteLine(Thread.CurrentThread.Name + " blocked: " + _counter + " >= " + _threshold);

            lock (_syncRoot)
            {
                while (_counter >= _threshold)
                {
                    _waiters++;
                    try
                    {
                        Monitor.Wait(_syncRoot);
                    }
                    catch (ThreadInterruptedException)
                    {
                        // Wait uninterruptably.
                    }
                    finally
                    {
                        _waiters--;
                    }
                }
            }

            Debug.WriteLine(Thread.CurrentThread.Name + " unblocked: " + _counter + " < " + _threshold);
        }

        protected void Unblock()
        {
            lock (_syncRoot)
            {
                if (_waiters > 0)
                {
                    Monitor.PulseAll(_syncRoot);
                }
            }
        }

        private Int32 EstimateSize(IoEvent ioe)
        {
            Int32 size = _sizeEstimator.EstimateSize(ioe);
            if (size < 0)
                throw new InvalidOperationException(_sizeEstimator.GetType().Name + " returned "
                        + "a negative value (" + size + "): " + ioe);
            return size;
        }
    }
}
