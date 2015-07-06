using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arch.CMessaging.Client.Net.Core.Filterchain;
using Arch.CMessaging.Client.Net.Core.Session;

namespace Arch.CMessaging.Client.Transport
{
    public class ExceptionHandler : IoFilterAdapter
    {
        public override void ExceptionCaught(INextFilter nextFilter, IoSession session, Exception cause)
        {
            base.ExceptionCaught(nextFilter, session, cause);
        }
    }
}
