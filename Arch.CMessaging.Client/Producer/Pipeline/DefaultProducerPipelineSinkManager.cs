using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arch.CMessaging.Client.Core.Ioc;
using Arch.CMessaging.Client.Core.Future;
using Arch.CMessaging.Client.Core.Result;
using Arch.CMessaging.Client.Core.Pipeline;
using Arch.CMessaging.Client.Core.MetaService;
using Arch.CMessaging.Client.Core.Utils;

namespace Arch.CMessaging.Client.Producer.Pipeline
{
    public class DefaultProducerPipelineSinkManager : IInitializable, IProducerPipelineSinkManager
    {
        private Dictionary<string, IPipelineSink> sinks;
        [Inject]
        private IMetaService meta;

        public DefaultProducerPipelineSinkManager()
        {
            this.sinks = new Dictionary<string, IPipelineSink>();
        }

        #region IInitializable Members

        public void Initialize()
        {
            foreach (var kvp in ComponentLocator.LookupMap<IPipelineSink>()) sinks[kvp.Key] = kvp.Value;
        }

        #endregion

        #region IProducerPipelineSinkManager Members

        public IPipelineSink GetSink(string topic)
        {
            var type = meta.FindEndpointTypeByTopic(topic);
            IPipelineSink sink = null;
            if (!sinks.TryGetValue(type, out sink))
                throw new ArgumentException(string.Format("Unknown message sink for topic {0}", topic));
            return sink;
        }

        #endregion
    }
}