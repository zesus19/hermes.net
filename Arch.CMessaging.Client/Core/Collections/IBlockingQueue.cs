using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arch.CMessaging.Client.Core.Collections
{
    public interface IBlockingQueue<TItem>
    {
        int Count { get; }
        bool Offer(TItem item);
        TItem Take();
        int DrainTo(IList<TItem> items);
        int DrainTo(IList<TItem> items, int maxElements);
    }
}
