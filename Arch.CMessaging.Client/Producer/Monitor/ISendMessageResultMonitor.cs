using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arch.CMessaging.Client.Transport.Command;

namespace Arch.CMessaging.Client.Producer.Monitor
{
    public interface ISendMessageResultMonitor
    {
        void Monitor(SendMessageCommand command);
        void ResultReceived(SendMessageResultCommand result);
    }
}
