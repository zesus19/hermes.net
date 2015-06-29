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

namespace Test
{
	public enum Xx
	{
		A,B
	}

	class MainClass
	{


		public static void Main (string[] args)
		{
			NameValueCollection config = ConfigurationManager.GetSection ("hermes/global") as NameValueCollection;
			Console.WriteLine(config["Hellox"] == null);
			Console.WriteLine (Enum.Parse(typeof(Xx), "A"));

			Dictionary<String, String> d = new Dictionary<String, String>();
			d.Add ("a", "b");
			Console.WriteLine(d["a"]);

			Regex x = new Regex("(\\d+),*");
			var matches = x.Matches ("[3,2,1]");
			foreach (Match m in matches) {
				Console.WriteLine(m.Groups[1]);
			}

		}


		public static void test() 

		{
			//Schema schema = Schema.Parse ("schema json");

			SpecificDatumWriter<User> w = new SpecificDatumWriter<User> (User._SCHEMA);
			User input = new User ();
			input.name = "mm";
			input.favorite_color = "red";
			input.favorite_number = 11;

			MemoryStream stream = new MemoryStream ();
			w.Write (input, new BinaryEncoder(stream));

			stream.Seek (0, SeekOrigin.Begin);

			SpecificDatumReader<User> r = new SpecificDatumReader<User>(User._SCHEMA, User._SCHEMA);
			User output = r.Read (null, new BinaryDecoder (stream));
			Console.WriteLine (output.name);
		}
	}
}
