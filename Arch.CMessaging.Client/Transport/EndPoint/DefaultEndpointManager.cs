﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arch.CMessaging.Client.Core.Ioc;
using Arch.CMessaging.Client.MetaEntity.Entity;
using Arch.CMessaging.Client.Core.MetaService;
using Arch.CMessaging.Client.MetaEntity.Entity;

namespace Arch.CMessaging.Client.Transport.EndPoint
{
    public class DefaultEndpointManager : IEndpointManager
    {
        [Inject]
        private IMetaService metaService;

        #region IEndpointManager Members

        public Endpoint GetEndpoint(string topic, int partition)
        {
            return metaService.FindEndpointByTopicAndPartition(topic, partition);
        }

        #endregion
    }
}
