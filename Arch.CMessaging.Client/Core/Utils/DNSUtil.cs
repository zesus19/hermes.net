using System;
using System.Collections.Generic;
using System.Net;

namespace Arch.CMessaging.Client.Core.Utils
{
	public class DNSUtil
	{
		public static List<string> resolve (String domainName)
		{
			List<string> result = new List<string> ();

			IPAddress[] addresses = Dns.GetHostAddresses (domainName);
			foreach (IPAddress a in addresses) {
				result.Add (a.ToString ());
			}

			return result;
		}
	}
}

