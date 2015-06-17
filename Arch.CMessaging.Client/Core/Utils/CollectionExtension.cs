using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arch.CMessaging.Client.Core.Utils
{
    public static class CollectionExtension
    {
        public static string AsString<TKey, TValue>(this Dictionary<TKey, TValue> map)
        {
            if (map == null || map.Count == 0) return "{}";
            else 
            {
                var sb = new StringBuilder();
                sb.Append("{");
                foreach (var kvp in map)
                {
                    if (kvp.Key != null && kvp.Value != null)
                        sb.Append(string.Format("{0}={1},", kvp.Key.ToString(), kvp.Value));
                }
                var str = sb.ToString().TrimEnd(',');
                return str += "}";
            }
        }
    }
}
