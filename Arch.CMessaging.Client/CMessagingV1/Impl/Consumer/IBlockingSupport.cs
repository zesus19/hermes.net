using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arch.CMessaging.Client.Core.Collections;
using Arch.CMessaging.Client.Event;

namespace Arch.CMessaging.Client.Impl.Consumer
{
    public interface IBlockingSupport
    {
        BlockingQueue<ConsumerCallbackEventArgs> BlockingQ { get; set; }
    }
}
