using System;
using System.Collections.Generic;

namespace Arch.CMessaging.Client.Core.Utils
{
	public class CollectionUtil
	{
		public static bool IsNullOrEmpty<T> (List<T> list)
		{
			return list == null || list.Count == 0;
		}

		public static V TryGet<K, V> (IDictionary<K, V> d, K key)
		{
			V result = default(V);
			d.TryGetValue (key, out result);
			return result;
		}
	}
}

