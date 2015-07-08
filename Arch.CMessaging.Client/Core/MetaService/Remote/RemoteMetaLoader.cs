using System;
using Arch.CMessaging.Client.MetaEntity.Entity;
using Arch.CMessaging.Client.Core.MetaService.Internal;
using Freeway.Logging;
using Arch.CMessaging.Client.Core.Env;
using Arch.CMessaging.Client.Core.Config;
using Arch.CMessaging.Client.Core.Utils;
using System.Collections.Generic;
using System.Net;
using Arch.CMessaging.Client.Core.Ioc;
using Arch.CMessaging.Client.Newtonsoft.Json;
using System.IO;
using System.Text;

namespace Arch.CMessaging.Client.Core.MetaService.Remote
{
    [Named(ServiceType = typeof(IMetaLoader), ServiceName = RemoteMetaLoader.ID)]
    public class RemoteMetaLoader : IMetaLoader
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(RemoteMetaLoader));

        public const String ID = "remote-meta-loader";

        [Inject]
        private IMetaServerLocator m_metaServerLocator;

        [Inject]
        private CoreConfig m_config;

        private ThreadSafe.AtomicReference<Meta> m_metaCache = new ThreadSafe.AtomicReference<Meta>(null);

        public Meta load()
        {
            List<string> ipPorts = m_metaServerLocator.getMetaServerList();
            if (ipPorts == null || ipPorts.Count == 0)
            {
                throw new Exception("No meta server found.");
            }

            foreach (string ipPort in ipPorts)
            {
                log.Debug(string.Format("Loading meta from server: {0}", ipPort));

                try
                {
                    string url = string.Format("http://{0}/meta", ipPort);
                    if (m_metaCache.ReadFullFence() != null)
                    {
                        url += "?version=" + m_metaCache.ReadFullFence().Version;
                    }

                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                    req.Timeout = m_config.MetaServerConnectTimeoutInMills + m_config.MetaServerReadTimeoutInMills;

                    using (HttpWebResponse res = (HttpWebResponse)req.BetterGetResponse())
                    {
                        HttpStatusCode statusCode = res.StatusCode;
                        if (statusCode == HttpStatusCode.OK)
                        {
                            using (var stream = new StreamReader(res.GetResponseStream(), Encoding.UTF8))
                            {
                                string responseContent = stream.ReadToEnd();
                                JsonSerializerSettings settings = new JsonSerializerSettings();
                                settings.NullValueHandling = NullValueHandling.Ignore;
                                m_metaCache.WriteFullFence(JsonConvert.DeserializeObject<Meta>(responseContent, settings));
                                return m_metaCache.ReadFullFence();
                            }
                        }
                        else if (statusCode == HttpStatusCode.NotModified)
                        {
                            return m_metaCache.ReadFullFence();
                        }
                    }

                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }
            throw new Exception(string.Format("Failed to load remote meta from {0}", ipPorts));
        }
    }
}

