using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arch.CMessaging.Client.Transport.Command;

namespace Arch.CMessaging.Client.Producer.Monitor
{
    public class DefaultSendMessageResultMonitor : ISendMessageResultMonitor
    {
        #region ISendMessageResultMonitor Members

        public void Monitor(SendMessageCommand cmd)
        {
            throw new NotImplementedException();
        }

        public void resultReceived(SendMessageResultCommand result)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
