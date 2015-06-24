using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arch.CMessaging.Client.Core.Collections
{
    public class ConsumeEventArgs : EventArgs
    {
        public ConsumeEventArgs(IConsumingItem item)
        {
            this.ConsumingItem = item;
        }

        public IConsumingItem ConsumingItem { get; private set; }
    }
}
