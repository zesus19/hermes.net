using System;
using System.Collections.Generic;

namespace Arch.CMessaging.Client.Core.MetaService.Remote
{
    public interface IMetaServerLocator
    {
        List<String> GetMetaServerList();
    }
}

