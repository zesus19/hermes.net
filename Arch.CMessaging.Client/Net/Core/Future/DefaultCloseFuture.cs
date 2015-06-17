using System;
using Arch.CMessaging.Client.Net.Core.Session;

namespace Arch.CMessaging.Client.Net.Core.Future
{
    public class DefaultCloseFuture : DefaultIoFuture, ICloseFuture
    {

        public DefaultCloseFuture(IoSession session)
            : base(session)
        { }

        public Boolean Closed
        {
            get
            {
                if (Done)
                {
                    Object v = Value;
                    if (v is Boolean)
                        return (Boolean)v;
                }
                return false;
            }
            set { Value = true; }
        }

        public new ICloseFuture Await()
        {
            return (ICloseFuture)base.Await();
        }
    }
}
