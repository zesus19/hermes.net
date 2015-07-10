using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arch.CMessaging.Client.Core.Collections
{
    public abstract class SortedAndIndexedBlockingQueue<TItem> : IIndexedBlockingQueue<SequentialNode<TItem>>
    {
        private int capacity;
        private RedBlack redBlack;
        private Dictionary<object, SequentialNode<TItem>> nodeMap;
        private object syncRoot = new object();

        public SortedAndIndexedBlockingQueue(int capacity)
        {
            this.capacity = capacity;
            this.redBlack = new RedBlack();
            this.nodeMap = new Dictionary<object, SequentialNode<TItem>>();
        }

        public int Count { get { return nodeMap.Count; } }

        public bool TryFind(object key, out SequentialNode<TItem> node)
        {
            return nodeMap.TryGetValue(key, out node);
        }

        public bool TryRemove(object key, out SequentialNode<TItem> node)
        {
            lock (syncRoot)
            {
                node = null;
                var removeOk = false;
                if (nodeMap.TryGetValue(key, out node))
                {
                    if (node.Next == null && node.Previous == null)
                    {
                        var minVal = redBlack.GetMinValue() as SequentialNode<TItem>;
                        if (minVal.SortedKey == node.SortedKey) node.NotifySequenceChanged();
                        redBlack.Remove(node.SortedKey);
                    }
                    else
                    {
                        if (node.Next == null) node.Previous.Next = null;
                        else if (node.Previous == null) node.Next.Previous = null;
                        else
                        {
                            node.Previous.Next = node.Next;
                            node.Next.Previous = node.Previous;
                        }
                    }
                    nodeMap.Remove(key);
                    removeOk = true;
                }
                return removeOk;
            }            
        }

        public bool Offer(SequentialNode<TItem> node)
        {
            return Offer(Guid.NewGuid().ToString(), node);
        }

        public bool Offer(object key, SequentialNode<TItem> node)
        {
            lock (syncRoot)
            {
                var offerOk = false;
                if (node == null) return false;
                if (nodeMap.Count == capacity) return false;
                if (!nodeMap.ContainsKey(key))
                {
                    object val = null;
                    nodeMap[key] = node;
                    if (redBlack.TryGetValue(node.SortedKey, out val))
                    {
                        var alreadyExists = val as SequentialNode<TItem>;
                        while (alreadyExists.Next != null)
                        {
                            alreadyExists = alreadyExists.Next;
                        }
                        alreadyExists.Next = node;
                        node.Previous = alreadyExists;
                    }
                    else
                    {
                        if (redBlack.Size() > 0)
                        {
                            var minVal = redBlack.GetMinValue() as SequentialNode<TItem>;
                            if (minVal.SortedKey.CompareTo(node.SortedKey) > 0) minVal.NotifySequenceChanged();
                        }
                        redBlack.Add(node.SortedKey, node);
                    }
                    Monitor.Pulse(syncRoot);
                    offerOk = true;
                }
                return offerOk;
            }
        }

        public SequentialNode<TItem> Take()
        {
            SequentialNode<TItem> node = null;
            SequentialNode<TItem> minValueItem = null;
            lock (syncRoot)
            {
                if (nodeMap.Count == 0) Monitor.Wait(syncRoot);
            }
            while (true)
            {
                lock (syncRoot)
                {
                    minValueItem = node = redBlack.GetMinValue() as SequentialNode<TItem>;
                }
                if (minValueItem.CheckIfTakeOk())
                {
                    do
                    {
                        SequentialNode<TItem> deleteNode;
                        TryRemove(minValueItem.Key, out deleteNode);
                        minValueItem = minValueItem.Next;
                    }
                    while (minValueItem != null);
                    break;
                }
            }
            return node;
        }

        #region IBlockingQueue<SequentialNode<TItem>> Members


        int IBlockingQueue<SequentialNode<TItem>>.DrainTo(IList<SequentialNode<TItem>> items)
        {
            lock(syncRoot)
            {
                return ((IBlockingQueue<SequentialNode<TItem>>)this).DrainTo(items, nodeMap.Count);
            }
        }

        int IBlockingQueue<SequentialNode<TItem>>.DrainTo(IList<SequentialNode<TItem>> items, int maxElements)
        {
            int count = 0;
            SequentialNode<TItem> minValueItem = null;
            lock (syncRoot)
            {
                while (count <= maxElements)
                {
                    if (redBlack.Size() == 0) break;
                    minValueItem = redBlack.GetMinValue() as SequentialNode<TItem>;
                    if (minValueItem == null) break;
                    else
                    {
                        do
                        {
                            SequentialNode<TItem> deleteNode;
                            TryRemove(minValueItem.Key, out deleteNode);
                            items.Add(minValueItem);
                            nodeMap.Remove(minValueItem.Key);
                            minValueItem = minValueItem.Next;
                            count++;
                        }
                        while (minValueItem != null);
                    }
                }
            }
            return count;
        }

        #endregion

        public int DrainTo(IList<TItem> items)
        {
            lock (syncRoot)
            {
                return DrainTo(items, nodeMap.Count);
            }
        }

        public int DrainTo(IList<TItem> items, int maxElements)
        {
            int count = 0;
            SequentialNode<TItem> minValueItem = null;
            lock (syncRoot)
            {
                while (count <= maxElements)
                {
                    if (redBlack.Size() == 0) break;
                    minValueItem = redBlack.GetMinValue() as SequentialNode<TItem>;
                    if (minValueItem == null) break;
                    else
                    {
                        do
                        {
                            SequentialNode<TItem> deleteNode;
                            TryRemove(minValueItem.Key, out deleteNode);
                            items.Add(minValueItem.Item);
                            nodeMap.Remove(minValueItem.Key);
                            minValueItem = minValueItem.Next;
                            count++;
                        }
                        while (minValueItem != null);
                    }
                }
            }
            return count;
        }
    }
}
