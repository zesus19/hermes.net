using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arch.CMessaging.Client.Core.Utils;
using Arch.CMessaging.Client.Core.Ioc;

namespace Arch.CMessaging.Client.Core.Message.Partition
{
	[Named (ServiceType = typeof(IPartitioningStrategy))]
	public class HashPartitioningStrategy : IPartitioningStrategy
	{
		private Random random = new Random ();

		#region IPartitioningStrategy Members

		public int ComputePartitionNo (string key, int partitionCount)
		{
			if (string.IsNullOrEmpty (key))
				return Math.Abs (random.Next ()) % partitionCount;
			else
				return Math.Abs (key.GetHashCode2 ()) % partitionCount;
		}

		#endregion
	}
}
