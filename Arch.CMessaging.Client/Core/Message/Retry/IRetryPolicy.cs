using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arch.CMessaging.Client.Core.Message.Retry
{
    public interface IRetryPolicy
    {
        int GetRetryTimes();
        long NextScheduleTimeMillis(int retryTimes, long currentTimeMillis);
    }
}
