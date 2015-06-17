using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Arch.CMessaging.Client.Net.Util
{
    class Pool<T>
    {
        ConcurrentStack<T> m_pool;
        public Pool()
        {
            m_pool = new ConcurrentStack<T>();
        }

        public Pool(IEnumerable<T> collection)
        {
            m_pool = new ConcurrentStack<T>(collection);
        }

        public void Push(T item)
        {
            if (item == null) { throw new ArgumentNullException("item", "Items added to a SocketAsyncEventArgsPool cannot be null"); }
            m_pool.Push(item);
        }

        public T Pop()
        {
            T e;
            m_pool.TryPop(out e);
            return e;
        }

        public int Count
        {
            get { return m_pool.Count; }
        }
    }
}
