using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arch.CMessaging.Client.Core.Service
{
    public interface ISystemClockService
    {
        long Now();
    }
}
