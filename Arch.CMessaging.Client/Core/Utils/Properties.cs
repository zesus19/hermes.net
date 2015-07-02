using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arch.CMessaging.Client.Core.Utils
{
	public class Properties
	{
		private Dictionary<string, string> dict;

		public Properties ()
		{
			dict = new Dictionary<string, string> ();
		}

		public bool ContainsKey (string key)
		{
			return dict.ContainsKey (key);
		}

		public string GetProperty (string key)
		{
			string result = null;
			dict.TryGetValue (key, out result);
			return result;
		}

		public string GetProperty (string key, string defaultValue)
		{
			if (ContainsKey (key)) {
				return GetProperty (key);
			} else {
				return defaultValue;
			}
		}

		public void SetProperty (string key, string value)
		{
			if (!string.IsNullOrEmpty (key)) {
				dict [key] = value;
			}
		}

	}
}