using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arch.CMessaging.Client.Core.Message.Partition
{
    public interface IPartitioningStrategy
    {
        int ComputePartitionNo(string key, int partitionCount);
    }
}
