using System;
using Arch.CMessaging.Client.Net.Core.Session;
using Arch.CMessaging.Client.Net.Core.Write;

namespace Arch.CMessaging.Client.Net.Core.Filterchain
{
    public abstract class IoFilterAdapter : IoFilter
    {
        
        public virtual void Init()
        { }

        
        public virtual void Destroy()
        { }

        
        public virtual void OnPreAdd(IoFilterChain parent, String name, INextFilter nextFilter)
        { }

        
        public virtual void OnPostAdd(IoFilterChain parent, String name, INextFilter nextFilter)
        { }

        
        public virtual void OnPreRemove(IoFilterChain parent, String name, INextFilter nextFilter)
        { }

        
        public virtual void OnPostRemove(IoFilterChain parent, String name, INextFilter nextFilter)
        { }

        
        public virtual void SessionCreated(INextFilter nextFilter, IoSession session)
        {
            nextFilter.SessionCreated(session);
        }

        
        public virtual void SessionOpened(INextFilter nextFilter, IoSession session)
        {
            nextFilter.SessionOpened(session);
        }

        
        public virtual void SessionClosed(INextFilter nextFilter, IoSession session)
        {
            nextFilter.SessionClosed(session);
        }

        
        public virtual void SessionIdle(INextFilter nextFilter, IoSession session, IdleStatus status)
        {
            nextFilter.SessionIdle(session, status);
        }

        
        public virtual void ExceptionCaught(INextFilter nextFilter, IoSession session, Exception cause)
        {
            nextFilter.ExceptionCaught(session, cause);
        }
        
        
        public virtual void InputClosed(INextFilter nextFilter, IoSession session)
        {
            nextFilter.InputClosed(session);
        }

        
        public virtual void MessageReceived(INextFilter nextFilter, IoSession session, Object message)
        {
            nextFilter.MessageReceived(session, message);
        }

        
        public virtual void MessageSent(INextFilter nextFilter, IoSession session, IWriteRequest writeRequest)
        {
            nextFilter.MessageSent(session, writeRequest);
        }

        
        public virtual void FilterClose(INextFilter nextFilter, IoSession session)
        {
            nextFilter.FilterClose(session);
        }

        
        public virtual void FilterWrite(INextFilter nextFilter, IoSession session, IWriteRequest writeRequest)
        {
            nextFilter.FilterWrite(session, writeRequest);
        }
    }
}
