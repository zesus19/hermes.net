﻿using System;
using Arch.CMessaging.Client.Core.Ioc;
using Arch.CMessaging.Client.MetaEntity.Entity;

namespace Arch.CMessaging.Client.Core.MetaService.Internal
{
    [Named(ServiceType = typeof(IMetaLoader), ServiceName = LocalMetaLoader.ID)]
    public class LocalMetaLoader : IMetaLoader
    {

        public const String ID = "local-meta-loader";

        public Meta Load()
        {
            throw new NotImplementedException();
        }
    }
}

