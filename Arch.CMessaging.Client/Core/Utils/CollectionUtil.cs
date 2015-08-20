using System;
using System.Collections.Generic;

namespace Arch.CMessaging.Client.Core.Utils
{
    public static class CollectionUtil
    {
        public static bool IsNullOrEmpty<T>(List<T> list)
        {
            return list == null || list.Count == 0;
        }

        public static V TryGet<K, V>(IDictionary<K, V> d, K key)
        {
            V result = default(V);
            d.TryGetValue(key, out result);
            return result;
        }

        public static void Shuffle<T>(this List<T> list)
        {
            Random rnd = new Random();
            list.Sort((a, b) => rnd.Next() > rnd.Next() ? 1 : -1);
        }
    }
}

