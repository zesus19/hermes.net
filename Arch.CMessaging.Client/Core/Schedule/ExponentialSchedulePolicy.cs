using System;
using System.Threading;

namespace Arch.CMessaging.Client.Core.Schedule
{
    public class ExponentialSchedulePolicy : ISchedulePolicy
    {
        private int lastDelayTime;

        private int delayBase;

        private int delayUpperbound;

        public ExponentialSchedulePolicy(int delayBase, int delayUpperbound)
        {
            this.delayBase = delayBase;
            this.delayUpperbound = delayUpperbound;
        }

        public long Fail(bool shouldSleep)
        {
            int delayTime = lastDelayTime;

            if (delayTime == 0)
            {
                delayTime = delayBase;
            }
            else
            {
                delayTime = Math.Min(lastDelayTime << 1, delayUpperbound);
            }

            if (shouldSleep)
            {
                Thread.Sleep(delayTime);
            }

            lastDelayTime = delayTime;
            return delayTime;
        }

        public void Succeess()
        {
            lastDelayTime = 0;
        }
    }
}

