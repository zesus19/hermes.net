using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arch.CMessaging.Client.Core.Ioc;
using Arch.CMessaging.Client.Core.Message;
using Arch.CMessaging.Client.Core.Message.Codec;

namespace Arch.CMessaging.Client.Core.Utils
{
    public class ComponentLocator
    {
        static IVenusContainer container;
        static object syncRoot = new object();
        public static void DefineComponents(Action<IVenusContainer> define)
        {
            if (container == null)
            {
                lock (syncRoot)
                {
                    if (container == null)
                    {
                        container = new VenusContainer();
                        define(container);
                    }
                }
            }
        }

        public static T Lookup<T>()
        {
            T obj = default(T);
            if (typeof(T) == typeof(IMessageCodec)) obj = (T)(object)new DefaultMessageCodec();
            return obj;
        }

        public static T Lookup<T>(string name)
        {
            T @default = default(T);
            @default = container.Lookup<T>(name);
            return @default;
        }

        public static IDictionary<string, T> LookupMap<T>()
        {
            return container.LookupMap<T>();
        }
    }
}
