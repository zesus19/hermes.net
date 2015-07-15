using System;
using Arch.CMessaging.Client.Core.Lease;
using Arch.CMessaging.Client.Core.Bo;
using System.Collections.Generic;
using Arch.CMessaging.Client.Core.MetaService.Internal;
using System.Net;
using Arch.CMessaging.Client.Core.Config;
using Freeway.Logging;
using System.IO;
using System.Text;
using System.Web;
using Arch.CMessaging.Client.Core.Ioc;
using Arch.CMessaging.Client.Core.Utils;
using Arch.CMessaging.Client.Newtonsoft.Json;

namespace Arch.CMessaging.Client.Core.MetaService.Remote
{
    [Named(ServiceType = typeof(IMetaProxy), ServiceName = RemoteMetaProxy.ID)]
    public class RemoteMetaProxy : IMetaProxy
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(RemoteMetaProxy));

        private  const string HOST = "host";

        private  const string BROKER_PORT = "brokerPort";

        private  const string LEASE_ID = "leaseId";

        private  const string SESSION_ID = "sessionId";

        private  const string TOPIC = "topic";

        private  const string PARTITION = "partition";

        public const string ID = "remote";

        [Inject]
        private IMetaServerLocator m_metaServerLocator;

        [Inject]
        private CoreConfig m_config;

        public LeaseAcquireResponse tryAcquireConsumerLease(Tpg tpg, String sessionId)
        {
            Dictionary<string, string> httppParams = new Dictionary<string, string>();
            httppParams.Add(SESSION_ID, sessionId);
            httppParams.Add(HOST, Local.IPV4);
            String response = post("/lease/consumer/acquire", httppParams, tpg);
            if (response != null)
            {
                LeaseAcquireResponse res = null;
                try
                {
                    res = JSON.DeserializeObject<LeaseAcquireResponse>(response);
                }
                catch (Exception e)
                {
                    Console.WriteLine(response);
                    Console.WriteLine(e);
                }
                return res;
            }
            else
            {
                return null;
            }
        }


        public LeaseAcquireResponse tryRenewConsumerLease(Tpg tpg, ILease lease, String sessionId)
        {
            Dictionary<string, string> p = new Dictionary<string, string>();
            p.Add(LEASE_ID, Convert.ToString(lease.ID));
            p.Add(SESSION_ID, sessionId);
            p.Add(HOST, Local.IPV4);
            String response = post("/lease/consumer/renew", p, tpg);
            if (response != null)
            {
                return JSON.DeserializeObject<LeaseAcquireResponse>(response);
            }
            else
            {
                return null;
            }
        }


        public LeaseAcquireResponse tryRenewBrokerLease(String topic, int partition, ILease lease, String sessionId,
                                                        int brokerPort)
        {
            throw new NotImplementedException();			

        }


        public LeaseAcquireResponse tryAcquireBrokerLease(String topic, int partition, String sessionId, int brokerPort)
        {
            throw new NotImplementedException();			
        }


        public List<SchemaView> listSchemas()
        {
            throw new NotImplementedException();			
        }


        public List<SubscriptionView> listSubscriptions()
        {
            throw new NotImplementedException();			
        }

        delegate string httpTo(string ipPort);

        private String post(string path, Dictionary<string, string> requestParams, Object payload)
        {
            return pollMetaServer((ipPort) =>
                {
                    UriBuilder uriBuilder = new UriBuilder("http://" + ipPort);
                    uriBuilder.Path = path;

                    var query = HttpUtility.ParseQueryString(string.Empty);
                    if (requestParams != null)
                    {
                        foreach (KeyValuePair<string, string> pair in requestParams)
                        {
                            query[pair.Key] = pair.Value;
                        }
                    }
                    uriBuilder.Query = query.ToString();

                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uriBuilder.ToString());
                    req.Method = "POST";
                    req.Timeout = m_config.MetaServerConnectTimeoutInMills + m_config.MetaServerReadTimeoutInMills;

                    if (payload != null)
                    {
                        req.ContentType = "application/json";

                        byte[] payloadJson = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload));
                        req.ContentLength = payloadJson.Length;
                        using (Stream reqStream = req.GetRequestStream())
                        {
                            reqStream.Write(payloadJson, 0, payloadJson.Length);
                        }
                    }

                    using (HttpWebResponse res = (HttpWebResponse)req.BetterGetResponse())
                    {

                        HttpStatusCode statusCode = res.StatusCode;
                        if (statusCode == HttpStatusCode.OK)
                        {
                            using (var stream = new StreamReader(res.GetResponseStream(), Encoding.UTF8))
                            {
                                return stream.ReadToEnd();
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }
                });
        }

        private String pollMetaServer(httpTo httpTo)
        {
            List<String> metaServerIpPorts = m_metaServerLocator.getMetaServerList();

            foreach (String ipPort in metaServerIpPorts)
            {
                String result = httpTo(ipPort);
                if (result != null)
                {
                    return result;
                }
                else
                {
                    continue;
                }
            }

            return null;

        }

        public int registerSchema(String schema, String subject)
        {
            Dictionary<String, String> p = new Dictionary<string, string>();
            p.Add("schema", schema);
            p.Add("subject", subject);
            String response = post("/schema/register", null, p);
            if (response != null)
            {
                try
                {
                    return Convert.ToInt32(response);
                }
                catch (Exception e)
                {
                    log.Warn(string.Format("Can not parse response, schema: {0}, subject: {1}\nResponse: {2}", schema, subject, response), e);
                }
            }
            else
            {
                log.Warn("No response while posting meta server[registerSchema]");
            }
            return -1;
        }

        public String getSchemaString(int schemaId)
        {
            throw new NotImplementedException();
        }

    }
}

