using System;
using Arch.CMessaging.Client.Newtonsoft.Json;
using System.Text;

namespace Arch.CMessaging.Client.Core.Utils
{
    public class JSON
    {
        private static JsonSerializerSettings settings;

        static JSON()
        {
            settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
        }

        public static T DeserializeObject<T>(String json)
        {
            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        public static T DeserializeObject<T>(byte[] json)
        {
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(json), settings);
        }

        public static object DeserializeObject(byte[] json, Type type)
        {
            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(json), type, settings);
        }

    }
}

