using System;
using Arch.CMessaging.Client.Core.Ioc;
using Freeway.Logging;
using Arch.CMessaging.Client.Core.Utils;
using Arch.CMessaging.Client.Core.Env;
using System.Collections.Generic;
using Arch.CMessaging.Client.Core.Config;
using System.Net;
using System.IO;
using System.Text;
using Arch.CMessaging.Client.Newtonsoft.Json;
using Arch.CMessaging.Client.Newtonsoft.Json.Linq;
using System.Threading;

namespace Arch.CMessaging.Client.Core.MetaService.Remote
{
    [Named(ServiceType = typeof(IMetaServerLocator))]
    public class DefaultMetaServerLocator : IMetaServerLocator, IInitializable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DefaultMetaServerLocator));

        private const int DEFAULT_MASTER_METASERVER_PORT = 80;

        [Inject]
        private IClientEnvironment clientEnv;

        [Inject]
        private CoreConfig coreConfig;


        private ThreadSafe.AtomicReference<List<string>> metaServerListRef = new ThreadSafe.AtomicReference<List<string>>(new List<string>());

        private int m_masterMetaServerPort = DEFAULT_MASTER_METASERVER_PORT;

        public Timer timer { get; set; }

        public MetaServerIpFetcher metaServerIpFetcher { get; set; }

        public List<String> GetMetaServerList()
        {
            return metaServerListRef.ReadFullFence();
        }

        public void UpdateMetaServerList()
        {
            if (CollectionUtil.IsNullOrEmpty(metaServerListRef.ReadFullFence()))
            {
                metaServerListRef.WriteFullFence(DomainToIpPorts());
            }

            metaServerListRef.WriteFullFence(FetchMetaServerListFromExistingMetaServer());
        }

        private List<String> FetchMetaServerListFromExistingMetaServer()
        {
            List<String> metaServerList = metaServerListRef.ReadFullFence();
            log.Debug(string.Format("Start fetching meta server ip from meta servers {0}", metaServerList));


            foreach (String ipPort in metaServerList)
            {
                try
                {
                    List<String> result = DoFetch(ipPort);
                    log.Debug(string.Format("Successfully fetched meta server ip from meta server {0}", ipPort));
                    return result;
                }
                catch (Exception)
                {
                    // ignore it

                }
            }

            throw new Exception("Failed to fetch meta server ip list from any meta server: " + metaServerList.ToString());
        }

        private List<string> DomainToIpPorts()
        {
            string domain = GetMetaServerDomainName();
            log.Info(string.Format("Meta server domain {0}", domain));
            try
            {
                List<string> ips = DNSUtil.resolve(domain);
                if (CollectionUtil.IsNullOrEmpty(ips))
                {
                    throw new Exception();
                }

                List<string> ipPorts = new List<string>();
                foreach (String ip in ips)
                {
                    ipPorts.Add(string.Format("{0}:{1}", ip, m_masterMetaServerPort));
                }

                return ipPorts;
            }
            catch (Exception e)
            {
                throw new Exception("Can not resolve meta server domain " + domain, e);
            }
        }

        private List<String> DoFetch(String ipPort)
        {
            string url = string.Format("http://{0}{1}", ipPort, "/metaserver/servers");

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Timeout = coreConfig.MetaServerConnectTimeoutInMills + coreConfig.MetaServerReadTimeoutInMills;

            using (var res = (HttpWebResponse)req.BetterGetResponse())
            {
                HttpStatusCode statusCode = res.StatusCode;
                if (statusCode == HttpStatusCode.OK)
                {
                    using (var reader = new StreamReader(res.GetResponseStream(), Encoding.UTF8))
                    {
                        string responseContent = reader.ReadToEnd();
                        JArray serverArray = (JArray)JsonConvert.DeserializeObject(responseContent);

                        List<string> servers = new List<string>();
                        for (int i = 0; i < serverArray.Count; i++)
                        {
                            servers.Add(serverArray[i].Value<string>());
                        }
                        return servers;
                    }
                }
                else
                {
                    throw new Exception(string.Format("HTTP code is {0} when fetch meta server list"));
                }
            }
        }

        private string GetMetaServerDomainName()
        {
            Arch.CMessaging.Client.Core.Env.Env env = clientEnv.GetEnv();

            switch (env)
            {
                case Arch.CMessaging.Client.Core.Env.Env.LOCAL:
                    return "meta.hermes.local";
                case Arch.CMessaging.Client.Core.Env.Env.DEV:
                    return "10.3.8.63";
                case Arch.CMessaging.Client.Core.Env.Env.LPT:
                    return "10.3.8.63";
                case Arch.CMessaging.Client.Core.Env.Env.FWS:
                    return "meta.hermes.fws.qa.nt.ctripcorp.com";
                case Arch.CMessaging.Client.Core.Env.Env.UAT:
                    return "meta.hermes.fx.uat.qa.nt.ctripcorp.com";
                case Arch.CMessaging.Client.Core.Env.Env.PROD:
                    return "meta.hermes.fx.ctripcorp.com";

                default:
                    throw new Exception(string.Format("Unknown hermes env {0}", env));
            }

        }

        public void Initialize()
        {
            if (clientEnv.IsLocalMode())
            {
                return;
            }

            m_masterMetaServerPort = DEFAULT_MASTER_METASERVER_PORT;

            string strMetaPort = clientEnv.GetGlobalConfig().GetProperty("meta.port");
            if (!string.IsNullOrEmpty(strMetaPort))
            {
                m_masterMetaServerPort = Convert.ToInt32(strMetaPort);
            }

            UpdateMetaServerList();

            int interval = (int)coreConfig.MetaServerIpFetchInterval * 1000;
            metaServerIpFetcher = new MetaServerIpFetcher(this, interval);
            timer = new Timer(metaServerIpFetcher.Fetch, null, interval, Timeout.Infinite);
        }

        public class MetaServerIpFetcher
        {
            private DefaultMetaServerLocator metaServerLocator;
            private int interval;

            public MetaServerIpFetcher(DefaultMetaServerLocator metaServerLocator, int interval)
            {
                this.metaServerLocator = metaServerLocator;
                this.interval = interval;
            }

            public void Fetch(object param)
            {
                metaServerLocator.timer.Change(Timeout.Infinite, Timeout.Infinite);
                try
                {
                    metaServerLocator.UpdateMetaServerList();
                }
                catch (Exception e)
                {
                    log.Warn("Error update meta server list", e);
                }
                finally
                {
                    metaServerLocator.timer.Change(interval, Timeout.Infinite);
                }
            }

        }
    }
}

