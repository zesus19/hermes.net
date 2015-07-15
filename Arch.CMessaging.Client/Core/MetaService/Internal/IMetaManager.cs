using System;
using Arch.CMessaging.Client.MetaEntity.Entity;

namespace Arch.CMessaging.Client.Core.MetaService.Internal
{
    public interface IMetaManager
    {
        Meta LoadMeta();

        IMetaProxy GetMetaProxy();
    }
}

