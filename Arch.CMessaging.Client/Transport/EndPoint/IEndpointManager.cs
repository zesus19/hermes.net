using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arch.CMessaging.Client.MetaEntity.Entity;

namespace Arch.CMessaging.Client.Transport.EndPoint
{
    public interface IEndpointManager
    {
        Endpoint GetEndpoint(string topic, int partition);
    }
}
