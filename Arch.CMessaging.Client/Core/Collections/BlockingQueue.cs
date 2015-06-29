using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Arch.CMessaging.Client.Core.Collections
{
    public class BlockingQueue<TItem> : IBlockingQueue<TItem>
    {
        private Queue<TItem> queue;
        private int capacity;
        private int waitCount;
        private object syncRoot = new object();

        public BlockingQueue(int capacity)
        {
            this.capacity = capacity;
            this.queue = new Queue<TItem>();
        }

        public int Count
        {
            get
            {
                lock (syncRoot) { return queue.Count; }
            }
        }

        public bool Offer(TItem item)
        {
            try
            {
                Monitor.Enter(syncRoot);
                if (queue.Count >= capacity) return false;
                queue.Enqueue(item);
                if (waitCount < 1) return true;
                waitCount--;
                Monitor.Pulse(syncRoot);
            }
            finally
            {
                Monitor.Exit(syncRoot);
            }
            return true;
        }


        public TItem Take()
        {
            TItem val;
            try
            {
                Monitor.Enter(syncRoot);
                if (queue.Count > 0)
                {
                    val = queue.Dequeue();
                }
                else
                {
                    waitCount++;
                    Monitor.Wait(syncRoot);
                    val = this.Take();
                }
            }
            finally
            {
                Monitor.Exit(syncRoot);
            }

            return val;
        }

        public int DrainTo(IList<TItem> items)
        {
            return DrainTo(items, capacity);
        }

        public int DrainTo(IList<TItem> items, int maxElements)
        {
            if (items == null) return 0;
            else
            {
                var idx = 0;
                try
                {
                    Monitor.Enter(syncRoot);
                    while (idx++ < maxElements && queue.Count > 0)
                    {
                        items.Add(queue.Dequeue());
                    }
                }
                finally
                {
                    Monitor.Exit(syncRoot);
                }
                return idx;
            }
        }

        public TItem Peek()
        {
            lock (syncRoot)
            {
                return queue.Count == 0 ? default(TItem) : queue.Peek();
            }
        }
    }
}
