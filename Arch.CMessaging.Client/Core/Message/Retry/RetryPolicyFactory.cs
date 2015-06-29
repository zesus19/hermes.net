using System;

namespace Arch.CMessaging.Client.Core.Message.Retry
{
	public class RetryPolicyFactory
	{
		public static IRetryPolicy create (String policyValue)
		{
			return null;
			/*
			if (policyValue != null) {
				if (policyValue.indexOf (":") != -1) {
					String[] splits = policyValue.split (":");
					if (splits != null && splits.length == 2) {
						String type = splits [0];
						String value = splits [1];

						if (type != null && !"".equals (type.trim ()) && value != null && !"".equals (value.trim ())) {

							switch (type.trim ()) {
							case "1":
								return new FrequencySpecifiedRetryPolicy (value.trim ());

							default:
								break;
							}
						}
					}
				}
			}
			throw new Exception (string.Format ("Unknown retry policy for value {0}", policyValue));
			*/
		}
	}
}

