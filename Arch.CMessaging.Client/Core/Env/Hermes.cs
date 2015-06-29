using System;

namespace Arch.CMessaging.Client.Core.Env
{
	public class Hermes
	{
		private static Env? m_env;

		public static void initialize (Env env)
		{
			m_env = env;
		}

		public static Env? getEnv ()
		{
			return m_env;
		}
	}
}

