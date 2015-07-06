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

namespace Test
{
	public enum Xx
	{
		A,
		B
	}


	class MainClass
	{

		public Task task{ get; set; }

		public Timer t { get; set; }

		public class Task
		{

			private MainClass mc;

			public Task (MainClass mc)
			{
				this.mc = mc;
			}

			public void go (object param)
			{
				Console.WriteLine ("go");
				mc.t.Dispose ();

				mc.t = new Timer (mc.task.go, null, 1000, 1000);
			}
		}

		public MainClass ()
		{
			task = new Task (this);
			t = new Timer (task.go, null, 1000, 1000);
		}

		public static void fetchMeta()
		{
			string url = "http://meta.hermes.fws.qa.nt.ctripcorp.com/meta";
			HttpWebRequest req = (HttpWebRequest)WebRequest.Create (url);
			req.Timeout = 5000;

			HttpWebResponse res = (HttpWebResponse)req.GetResponse ();

			HttpStatusCode statusCode = res.StatusCode;
			if (statusCode == HttpStatusCode.OK) {
				string responseContent = new StreamReader (res.GetResponseStream (), Encoding.UTF8).ReadToEnd ();
				JsonSerializerSettings settings = new JsonSerializerSettings();
				settings.NullValueHandling = NullValueHandling.Ignore;
				Meta meta = JsonConvert.DeserializeObject<Meta> (responseContent, settings);
				Console.WriteLine (JsonConvert.SerializeObject (meta));
			} else if (statusCode == HttpStatusCode.NotModified) {
			}
		}

        public static void Send ()
        {
            ComponentsConfigurator.DefineComponents ();
            var p = Arch.CMessaging.Client.Producer.Producer.GetInstance ();
            var future = p.Message ("order_new", "", "hello c#").Send ();
            var result = future.Get();
            Console.WriteLine("aaa");
            Console.ReadLine ();
        }

        public class A<T>
        {
        }

		public static void Main (string[] args)
		{
            Send();
		}

		public static void test2 ()
		{
			NameValueCollection config = ConfigurationManager.GetSection ("hermes/global") as NameValueCollection;
			Console.WriteLine (config ["Hellox"] == null);
			Console.WriteLine (Enum.Parse (typeof(Xx), "A"));

			Dictionary<String, String> d = new Dictionary<String, String> ();
			d.Add ("a", "b");
			Console.WriteLine (d ["a"]);

			Regex x = new Regex ("(\\d+),*");
			var matches = x.Matches ("[3,2,1]");
			foreach (Match m in matches) {
				Console.WriteLine (m.Groups [1]);
			}

		}


		public static void test ()
		{
			//Schema schema = Schema.Parse ("schema json");

			SpecificDatumWriter<User> w = new SpecificDatumWriter<User> (User._SCHEMA);
			User input = new User ();
			input.name = "mm";
			input.favorite_color = "red";
			input.favorite_number = 11;

			MemoryStream stream = new MemoryStream ();
			w.Write (input, new BinaryEncoder (stream));

			stream.Seek (0, SeekOrigin.Begin);

			SpecificDatumReader<User> r = new SpecificDatumReader<User> (User._SCHEMA, User._SCHEMA);
			User output = r.Read (null, new BinaryDecoder (stream));
			Console.WriteLine (output.name);
		}
	}
}
