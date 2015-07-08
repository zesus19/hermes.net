using System;
using Avro.Specific;
using example.avro;
using Avro.IO;
using System.IO;
using Avro;
using System.Configuration;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using Arch.CMessaging.Client.MetaEntity.Entity;
using Arch.CMessaging.Client.Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Web;
using System.Collections;
using Arch.CMessaging.Client.Newtonsoft.Json.Linq;
using Arch.CMessaging.Client.Core.Utils;
using Arch.CMessaging.Client.Producer.Build;
using Arch.CMessaging.Client.Consumer;
using Arch.CMessaging.Client.Consumer.Api;
using Arch.CMessaging.Client.Core.Message;

namespace Test
{
    class MainClass
    {
        public static void fetchMeta()
        {
            string url = "http://meta.hermes.fws.qa.nt.ctripcorp.com/meta";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Timeout = 5000;

            HttpWebResponse res = (HttpWebResponse)req.GetResponse();

            HttpStatusCode statusCode = res.StatusCode;
            if (statusCode == HttpStatusCode.OK)
            {
                string responseContent = new StreamReader(res.GetResponseStream(), Encoding.UTF8).ReadToEnd();
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.NullValueHandling = NullValueHandling.Ignore;
                Meta meta = JsonConvert.DeserializeObject<Meta>(responseContent, settings);
                Console.WriteLine(JsonConvert.SerializeObject(meta));
            }
            else if (statusCode == HttpStatusCode.NotModified)
            {
            }
        }

        public static void Main(string[] args)
        {
            //Consume();
            Produce();
        }

        class MyConsumer : BaseMessageListener
        {
            public MyConsumer(string groupId)
                : base(groupId)
            {
            }

            protected override void OnMessage(IConsumerMessage msg)
            {
                Console.WriteLine(msg.GetBody<string>());
            }

            public override Type MessageType()
            {
                return typeof(string);
            }
        }

        public static void Consume()
        {
            ComponentsConfigurator.DefineComponents();
            Consumer.GetInstance().Start("order_new", "group1", new MyConsumer("group1"));
        }

        public static void Produce()
        {
            ComponentsConfigurator.DefineComponents();
            var p = Arch.CMessaging.Client.Producer.Producer.GetInstance();
            int cnt = 0;
            long start = new DateTime().CurrentTimeMillis();
            while (cnt++ < 10000)
            {
                try
                {
                    var future = p.Message("order_new", "", "hello c#").Send();
                    //var result = future.Get(2000);
                    //Console.WriteLine("aaa");
                    //Thread.Sleep(1000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            Console.WriteLine(new DateTime().CurrentTimeMillis() - start);
        }

        public static void test2()
        {
            NameValueCollection config = ConfigurationManager.GetSection("hermes/global") as NameValueCollection;
            Console.WriteLine(config["Hellox"] == null);

            Dictionary<String, String> d = new Dictionary<String, String>();
            d.Add("a", "b");
            Console.WriteLine(d["a"]);

            Regex x = new Regex("(\\d+),*");
            var matches = x.Matches("[3,2,1]");
            foreach (Match m in matches)
            {
                Console.WriteLine(m.Groups[1]);
            }

        }


        public static void test()
        {
            //Schema schema = Schema.Parse ("schema json");

            SpecificDatumWriter<User> w = new SpecificDatumWriter<User>(User._SCHEMA);
            User input = new User();
            input.name = "mm";
            input.favorite_color = "red";
            input.favorite_number = 11;

            MemoryStream stream = new MemoryStream();
            w.Write(input, new BinaryEncoder(stream));

            stream.Seek(0, SeekOrigin.Begin);

            SpecificDatumReader<User> r = new SpecificDatumReader<User>(User._SCHEMA, User._SCHEMA);
            User output = r.Read(null, new BinaryDecoder(stream));
            Console.WriteLine(output.name);
        }
    }
}
