using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arch.CMessaging.Client.Core.Utils
{
    public static class StringExtension
    {
        public static int GetHashCode2(this string val)
        {
            if (string.IsNullOrEmpty(val)) return string.Empty.GetHashCode();
            else
            {
                var hash = 0;
                var chars = val.ToCharArray();
                for (int i = 0; i < chars.Length; i++) hash = 31 * hash + chars[i];
                return hash;
            }
        }
    }
}
