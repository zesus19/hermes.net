using System;

namespace Arch.CMessaging.Client.Core.Schedule
{
    public interface ISchedulePolicy
    {
        long Fail(bool shouldSleep);

        void Succeess();
    }
}

