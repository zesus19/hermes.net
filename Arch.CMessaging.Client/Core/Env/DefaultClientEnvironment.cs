using System;
using Freeway.Logging;
using System.Collections.Generic;
using Arch.CMessaging.Client.Core.Utils;
using System.Collections.Concurrent;
using System.Threading;
using System.Configuration;
using System.Collections.Specialized;
using Arch.CMessaging.Client.Core.Ioc;

namespace Arch.CMessaging.Client.Core.Env
{
    [Named(ServiceType = typeof(IClientEnvironment))]
    public class DefaultClientEnvironment : IClientEnvironment, IInitializable
    {
        private static String PRODUCER_DEFAULT_FILE = "/hermes-producer.properties";

        private  static String PRODUCER_PATTERN = "/hermes-producer-{0}.properties";

        private  static String CONSUMER_DEFAULT_FILE = "/hermes-consumer.properties";

        private  static String CONSUMER_PATTERN = "/hermes-consumer-{0}.properties";

        private  static String GLOBAL_DEFAULT_FILE = "/hermes.properties";

        private static  String KEY_IS_LOCAL_MODE = "isLocalMode";

        private ConcurrentDictionary<String, Properties> ProducerCache = new ConcurrentDictionary<String, Properties>();

        private ConcurrentDictionary<String, Properties> ConsumerCache = new ConcurrentDictionary<String, Properties>();

        private Properties producerDefault;

        private Properties consumerDefault;

        public Properties GlobalDefault { get; set; }

        private static readonly ILog log = LogManager.GetLogger(typeof(DefaultClientEnvironment));

        private volatile Env env;

        private Dictionary<Env, string> env2MetaDomain = new Dictionary<Env, String>();

        public string getMetaServerDomainName()
        {
            return env2MetaDomain[GetEnv()];
        }

        public Properties GetGlobalConfig()
        {
            return GlobalDefault;
        }

        public Properties GetProducerConfig(String topic)
        {
            Properties properties;
            ProducerCache.TryGetValue(topic, out properties);
            if (properties == null)
            {
                properties = readConfigFile(String.Format(PRODUCER_PATTERN, topic), producerDefault);
                ProducerCache.GetOrAdd(topic, properties);
            }

            return properties;
        }

        public Properties GetConsumerConfig(String topic)
        {
            Properties properties;
            ConsumerCache.TryGetValue(topic, out properties);
            if (properties == null)
            {
                properties = readConfigFile(String.Format(CONSUMER_PATTERN, topic), consumerDefault);
                ConsumerCache.GetOrAdd(topic, properties);
            }

            return properties;
        }

        private Properties readConfigFile(String configPath)
        {
            return readConfigFile(configPath, null);
        }

        private Properties readConfigFile(String configPath, Properties defaults)
        {
            if (GLOBAL_DEFAULT_FILE.Equals(configPath))
            {
                var globalProperties = new Properties();
                NameValueCollection config = ConfigurationManager.GetSection("hermes/global") as NameValueCollection;
                if (config != null)
                {
                    foreach (string k in config)
                    {
                        globalProperties.SetProperty(k, config[k]);
                    }
                }

                return globalProperties;
            }

            // TODO support read config file
            return new Properties();
        }

		
        public void Initialize()
        {
            producerDefault = readConfigFile(PRODUCER_DEFAULT_FILE);
            consumerDefault = readConfigFile(CONSUMER_DEFAULT_FILE);
            GlobalDefault = readConfigFile(GLOBAL_DEFAULT_FILE);

            Env? resultEnv = Hermes.GetEnv();

            NameValueCollection config = ConfigurationManager.GetSection("hermes/global") as NameValueCollection;
            if (config != null && config["env"] != null)
            {
                Env newEnv = (Env)Enum.Parse(typeof(Env), config["env"].ToUpper());
                if (resultEnv != null && newEnv != resultEnv)
                {
                    throw new Exception(string.Format("Inconsist Hermes env {0} {1}", resultEnv, newEnv));
                }
                else
                {
                    resultEnv = newEnv;
                }
            }

            if (resultEnv == null)
            {
                throw new Exception("Hermes env is not set");
            }

            env = resultEnv.Value;

            env2MetaDomain.Add(Env.LOCAL, GlobalDefault.GetProperty("local.domain", "meta.hermes.local"));
            // TODO use real dev&lpt domain when get dev&lpt domain
            env2MetaDomain.Add(Env.DEV, GlobalDefault.GetProperty("dev.domain", "10.3.8.63"));
            env2MetaDomain.Add(Env.LPT, GlobalDefault.GetProperty("lpt.domain", "10.2.5.133"));
            env2MetaDomain.Add(Env.FAT, GlobalDefault.GetProperty("fat.domain", "meta.hermes.fws.qa.nt.ctripcorp.com"));
            env2MetaDomain.Add(Env.FWS, GlobalDefault.GetProperty("fws.domain", "meta.hermes.fws.qa.nt.ctripcorp.com"));
            env2MetaDomain
                .Add(Env.UAT, GlobalDefault.GetProperty("uat.domain", "meta.hermes.fx.uat.qa.nt.ctripcorp.com"));
            env2MetaDomain.Add(Env.PROD, GlobalDefault.GetProperty("prod.domain", "meta.hermes.fx.ctripcorp.com"));

            log.Info(string.Format("Meta server domains: {0}", env2MetaDomain));
        }

        public Env GetEnv()
        {
            return env;
        }

		
        public bool IsLocalMode()
        {
            bool isLocalMode;
            if (GetGlobalConfig().ContainsKey(KEY_IS_LOCAL_MODE))
            {
                isLocalMode = Convert.ToBoolean(GetGlobalConfig().GetProperty(KEY_IS_LOCAL_MODE));
            }
            else
            {
                isLocalMode = false;
            }
            return isLocalMode;
        }

    }
}