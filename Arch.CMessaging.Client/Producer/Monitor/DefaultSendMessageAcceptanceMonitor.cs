using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arch.CMessaging.Client.Core.Future;

namespace Arch.CMessaging.Client.Producer.Monitor
{
    public class DefaultSendMessageAcceptanceMonitor : ISendMessageAcceptanceMonitor
    {
        #region ISendMessageAcceptanceMonitor Members

        public IFuture<bool> Monitor(long correlationID)
        {
            throw new NotImplementedException();
        }

        public void Recieved(long correlationID, bool success)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
