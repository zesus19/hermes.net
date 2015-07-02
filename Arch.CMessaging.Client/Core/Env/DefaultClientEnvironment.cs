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
	[Named (ServiceType = typeof(IClientEnvironment))]
	public class DefaultClientEnvironment : IClientEnvironment, IInitializable
	{
		private static String PRODUCER_DEFAULT_FILE = "/hermes-producer.properties";

		private  static String PRODUCER_PATTERN = "/hermes-producer-{0}.properties";

		private  static String CONSUMER_DEFAULT_FILE = "/hermes-consumer.properties";

		private  static String CONSUMER_PATTERN = "/hermes-consumer-{0}.properties";

		private  static String GLOBAL_DEFAULT_FILE = "/hermes.properties";

		private static  String KEY_IS_LOCAL_MODE = "isLocalMode";

		private ConcurrentDictionary<String, Properties> ProducerCache = new ConcurrentDictionary<String, Properties> ();

		private ConcurrentDictionary<String, Properties> ConsumerCache = new ConcurrentDictionary<String, Properties> ();

		private Properties m_producerDefault;

		private Properties m_consumerDefault;

		public Properties m_globalDefault { get; set; }

		private static readonly ILog log = LogManager.GetLogger (typeof(DefaultClientEnvironment));

		private volatile Env m_env;


		public Properties GetGlobalConfig ()
		{
			return m_globalDefault;
		}

		public Properties GetProducerConfig (String topic)
		{
			Properties properties = ProducerCache [topic];
			if (properties == null) {
				properties = readConfigFile (String.Format (PRODUCER_PATTERN, topic), m_producerDefault);
				ProducerCache.GetOrAdd (topic, properties);
			}

			return properties;
		}

		public Properties GetConsumerConfig (String topic)
		{
			Properties properties = ConsumerCache [topic];
			if (properties == null) {
				properties = readConfigFile (String.Format (CONSUMER_PATTERN, topic), m_consumerDefault);
				ConsumerCache.GetOrAdd (topic, properties);
			}

			return properties;
		}

		private Properties readConfigFile (String configPath)
		{
			return readConfigFile (configPath, null);
		}

		private Properties readConfigFile (String configPath, Properties defaults)
		{

            //if (GLOBAL_DEFAULT_FILE.Equals (configPath)) {
            //    var globalProperties = new Properties ();
            //    NameValueCollection config = ConfigurationManager.GetSection ("hermes/global") as NameValueCollection;
            //    if (config != null) {
            //        foreach (string k in config) {
            //            globalProperties.SetProperty (k, config [k]);
            //        }
            //    }

            //    return globalProperties;
            //}

			// TODO support read config file
			return new Properties ();
		}

		
		public void Initialize ()
		{
			m_producerDefault = readConfigFile (PRODUCER_DEFAULT_FILE);
			m_consumerDefault = readConfigFile (CONSUMER_DEFAULT_FILE);
			m_globalDefault = readConfigFile (GLOBAL_DEFAULT_FILE);

			Env? resultEnv = Hermes.getEnv ();

            //NameValueCollection config = ConfigurationManager.GetSection ("hermes/global") as NameValueCollection;
            //if (config != null && config ["env"] != null) {
            //    Env newEnv = (Env)Enum.Parse (typeof(Env), config ["env"].ToUpper ());
            //    if (resultEnv != null && newEnv != resultEnv) {
            //        throw new Exception (string.Format ("Inconsist Hermes env {0} {1}", resultEnv, newEnv));
            //    } else {
            //        resultEnv = newEnv;
            //    }
            //}

            //if (resultEnv == null) {
            //    throw new Exception ("Hermes env is not set");
            //}

			m_env = Env.LOCAL;
		}

		
		public Env GetEnv ()
		{
			return m_env;
		}

		
		public bool IsLocalMode ()
		{
			bool isLocalMode;
			if (GetGlobalConfig ().ContainsKey (KEY_IS_LOCAL_MODE)) {
				isLocalMode = Convert.ToBoolean (GetGlobalConfig ().GetProperty (KEY_IS_LOCAL_MODE));
			} else {
				isLocalMode = false;
			}
			return isLocalMode;
		}

	}
}