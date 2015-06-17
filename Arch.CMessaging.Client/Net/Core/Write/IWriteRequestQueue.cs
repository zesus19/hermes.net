using System;
using Arch.CMessaging.Client.Net.Core.Session;

namespace Arch.CMessaging.Client.Net.Core.Write
{
    public interface IWriteRequestQueue
    {
        IWriteRequest Poll(IoSession session);
        void Offer(IoSession session, IWriteRequest writeRequest);
        Boolean IsEmpty(IoSession session);
        void Clear(IoSession session);
        void Dispose(IoSession session);
        Int32 Size { get; }
    }
}
