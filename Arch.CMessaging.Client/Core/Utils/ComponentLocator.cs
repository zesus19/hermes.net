using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arch.CMessaging.Client.Core.Message;
using Arch.CMessaging.Client.Core.Message.Codec;

namespace Arch.CMessaging.Client.Core.Utils
{
    public class ComponentLocator
    {
        public static T Lookup<T>()
        {
            T obj = default(T);
            if (typeof(T) == typeof(IMessageCodec)) obj = (T)(object)new DefaultMessageCodec();
            return obj;
        }
    }
}
