using System;

namespace Arch.CMessaging.Client.Core.Env
{
    public class Hermes
    {
        private static Env? m_env;

        public static void Initialize(Env env)
        {
            m_env = env;
        }

        public static Env? GetEnv()
        {
            return m_env;
        }
    }
}

