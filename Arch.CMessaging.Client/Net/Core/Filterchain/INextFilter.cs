using System;
using Arch.CMessaging.Client.Net.Core.Session;
using Arch.CMessaging.Client.Net.Core.Write;

namespace Arch.CMessaging.Client.Net.Core.Filterchain
{
    public interface INextFilter
    {       
        void SessionCreated(IoSession session);   
        void SessionOpened(IoSession session);
        void SessionClosed(IoSession session);
        void SessionIdle(IoSession session, IdleStatus status);
        void ExceptionCaught(IoSession session, Exception cause);
        void InputClosed(IoSession session);
        void MessageReceived(IoSession session, Object message);
        void MessageSent(IoSession session, IWriteRequest writeRequest);
        void FilterClose(IoSession session);
        void FilterWrite(IoSession session, IWriteRequest writeRequest);
    }
}
