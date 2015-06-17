using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arch.CMessaging.Client.Meta.Entity;
using Arch.CMessaging.Client.Transport.Command;

namespace Arch.CMessaging.Client.Transport.EndPoint
{
    public class DefaultEndpointClient : IEndpointClient
    {
        #region IEndpointClient Members

        public void WriteCommand(Endpoint endpoint, ICommand command)
        {
            throw new NotImplementedException();
        }

        public void WriteCommand(Endpoint endpoint, ICommand command, long timeoutInMills)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
