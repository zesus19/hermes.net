using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arch.CMessaging.Client.Core.Collections
{
    public class TimeoutNotifyQueue<TItem> : SortedAndIndexedBlockingQueue<TItem>
    {
        private AutoResetEvent waitHandle;
        public TimeoutNotifyQueue(int capacity)
            : base(capacity)
        {
            this.waitHandle = new AutoResetEvent(false);
        }

        public bool TryFind(object key, out TItem item)
        {
            var found = false;
            item = default(TItem);
            SequentialNode<TItem> node;
            if (base.TryFind(key, out node))
            {
                item = node.Item;
                found = true;
            }
            return found;
        }

        public bool TryRemove(object key, out TItem item)
        {
            var removeOk = false;
            item = default(TItem);
            SequentialNode<TItem> node;
            if (base.TryRemove(key, out node))
            {
                item = node.Item;
                removeOk = true;
            }
            return removeOk;
        }

        public bool Offer(object key, TItem item, int timeout)
        {
            return base.Offer(key, new TimeoutSequenceNode<TItem>(key, item, timeout, waitHandle));
        }

        private class TimeoutSequenceNode<T> : SequentialNode<T>
        {
            private long timeoutDueTicks;
            private bool waitingTimeout;
            private AutoResetEvent waitHandle;
            public TimeoutSequenceNode(object key, T item, int timeout, AutoResetEvent waitHandle)
                : base(key, item)
            {
                this.waitHandle = waitHandle;
                this.waitingTimeout = false;
                this.timeoutDueTicks = DateTime.Now.Ticks + TimeSpan.TicksPerMillisecond * timeout;
            }

            public override IComparable SortedKey
            {
                get { return timeoutDueTicks; }
            }

            public override bool CheckIfTakeOk()
            {
                var timedout = true;
                var waitTime = timeoutDueTicks - DateTime.Now.Ticks;
                // wait to timeout. when receives a signal during waiting, that means time out was broken.
                if (waitTime > 0)
                {
                    waitingTimeout = true;
                    timedout = !waitHandle.WaitOne(TimeSpan.FromTicks(waitTime));
                    waitingTimeout = false;
                }
                return timedout;
            }

            public override void NotifySequenceChanged()
            {
                if (waitingTimeout) waitHandle.Set();
            }
        }
    }
}
