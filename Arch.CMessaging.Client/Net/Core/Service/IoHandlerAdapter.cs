using System;
using Arch.CMessaging.Client.Net.Core.Session;
using System.Diagnostics;

namespace Arch.CMessaging.Client.Net.Core.Service
{
    public class IoHandlerAdapter : IoHandler
    {
        
        public virtual void SessionCreated(IoSession session)
        {
            // Empty handler
        }
        public virtual void SessionOpened(IoSession session)
        {
            // Empty handler
        }

        public virtual void SessionClosed(IoSession session)
        {
            // Empty handler
        }        
        public virtual void SessionIdle(IoSession session, IdleStatus status)
        {
            // Empty handler
        }

        public virtual void ExceptionCaught(IoSession session, Exception cause)
        {
            Debug.WriteLine("EXCEPTION, please implement {0}.ExceptionCaught() for proper handling: {1}", GetType().Name, cause);
        }

        public virtual void MessageReceived(IoSession session, Object message)
        {
            // Empty handler
        }
        
        public virtual void MessageSent(IoSession session, Object message)
        {
            // Empty handler
        }

        public void InputClosed(IoSession session)
        {
            session.Close(true);
        }
    }
}
