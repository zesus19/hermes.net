using System;

namespace Arch.CMessaging.Client.Core.Message.Retry
{
	public class RetryPolicyFactory
	{
		public static IRetryPolicy create (String policyValue)
		{
			if (policyValue != null) {
				if (policyValue.IndexOf (":") != -1) {
					String[] splits = policyValue.Split (new string[]{ ":" }, StringSplitOptions.None);
					if (splits != null && splits.Length == 2) {
						String type = splits [0];
						String value = splits [1];

						if (type != null && !"".Equals (type.Trim ()) && value != null && !"".Equals (value.Trim ())) {

							switch (type.Trim ()) {
							case "1":
								return new FrequencySpecifiedRetryPolicy (value.Trim ());

							default:
								break;
							}
						}
					}
				}
			}
			throw new Exception (string.Format ("Unknown retry policy for value {0}", policyValue));
		}
	}
}

