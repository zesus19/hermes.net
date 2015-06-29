using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arch.CMessaging.Client.MetaEntity.Entity;
using Arch.CMessaging.Client.Transport.Command;

namespace Arch.CMessaging.Client.Transport.EndPoint
{
    public interface IEndpointClient
    {
        void WriteCommand(Endpoint endpoint, ICommand command);
        void WriteCommand(Endpoint endpoint, ICommand command, int timeoutInMills);
    }
}
