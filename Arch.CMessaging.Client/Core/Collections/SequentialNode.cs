using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arch.CMessaging.Client.Core.Collections
{
    public abstract class SequentialNode<T> 
    {
        protected SequentialNode(object key, T item)
        {
            this.Key = key;
            this.Item = item;
        }

        public object Key { get; private set; }
        public T Item { get; private set; }
        public SequentialNode<T> Next { get; set; }
        public SequentialNode<T> Previous { get; set; }
        public abstract IComparable SortedKey { get; }
        public abstract bool CheckIfTakeOk();
        public abstract void NotifySequenceChanged();
    }
}
