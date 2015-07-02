using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arch.CMessaging.Client.Core.Future
{
    public class FutureCallbackItem<TItem>
    {
        public object Item { get; set; }
        public IFutureCallback<TItem> Callback { get; set; }
    }
}
