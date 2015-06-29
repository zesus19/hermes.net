using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Arch.CMessaging.Client.Core.Message.Retry
{
	public class FrequencySpecifiedRetryPolicy : IRetryPolicy
	{
		private static Regex PATTERN_VALID = new Regex ("\\[(\\d+,*)+\\]");

		private static Regex PATTERN_GROUP = new Regex ("(\\d+),*");

		private String m_policyValue;

		private int m_retryTimes;

		private List<int> m_intervals;

		public FrequencySpecifiedRetryPolicy (String policyValue)
		{
			if (policyValue == null) {
				throw new Exception ("Policy value can not be null");
			}

			m_policyValue = policyValue.Trim ();
			if (PATTERN_VALID.Match (m_policyValue).Success) {
				m_intervals = new List<int> ();
				MatchCollection matches = PATTERN_GROUP.Matches (m_policyValue.Substring (1, m_policyValue.Length - 2));
				foreach (Match m in matches) {
					m_intervals.Add (Convert.ToInt32 (m.Groups[1]));
				}
				m_retryTimes = m_intervals.Count;
			} else {
				throw new Exception (string.Format ("Policy value {0} is invalid", m_policyValue));
			}
		}

		public int GetRetryTimes ()
		{
			return m_retryTimes;
		}

		public long NextScheduleTimeMillis (int retryTimes, long currentTimeMillis)
		{
			if (retryTimes >= m_retryTimes) {
				return 0L;
			} else {
				return currentTimeMillis + m_intervals [retryTimes] * 1000;
			}
		}
	}
}

