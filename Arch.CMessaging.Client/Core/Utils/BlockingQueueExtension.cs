using System;
using Arch.CMessaging.Client.Core.Collections;
using System.Collections.Generic;

namespace Arch.CMessaging.Client.Core.Utils
{
    public static class BlockingQueueExtension
    {
        public static void AddAll<T>(this BlockingQueue<T> queue, ICollection<T> items)
        {
            if (items != null)
            {
                foreach (var item in items)
                {
                    queue.Offer(item);
                }
            }
        }
    }
}

