using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arch.CMessaging.Client.Core.Meta;
using Arch.CMessaging.Client.Meta.Entity;

namespace Arch.CMessaging.Client.Transport.EndPoint
{
    public class DefaultEndpointManager : IEndpointManager
    {
        //ioc inject
        private IMetaService metaService;

        #region IEndpointManager Members

        public Endpoint GetEndpoint(string topic, int partition)
        {
            return metaService.FindEndpointByTopicAndPartition(topic, partition);
        }

        #endregion
    }
}
