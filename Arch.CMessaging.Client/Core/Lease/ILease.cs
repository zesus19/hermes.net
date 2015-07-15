using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arch.CMessaging.Client.Core.Lease
{
    public interface ILease
    {
        long ID { get; }

        bool Expired { get; }

        long ExpireTime { get; set; }

        long RemainingTime { get; }
    }
}
