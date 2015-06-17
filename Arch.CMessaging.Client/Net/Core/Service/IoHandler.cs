using System;
using Arch.CMessaging.Client.Net.Core.Session;

namespace Arch.CMessaging.Client.Net.Core.Service
{
    public interface IoHandler
    {
        void SessionCreated(IoSession session);
        void SessionOpened(IoSession session);
        void SessionClosed(IoSession session);
        void SessionIdle(IoSession session, IdleStatus status);
        void ExceptionCaught(IoSession session, Exception cause);
        void MessageReceived(IoSession session, Object message);
        void MessageSent(IoSession session, Object message);
        void InputClosed(IoSession session);
    }
}
