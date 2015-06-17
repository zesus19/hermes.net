using System;
using System.Collections.Generic;

namespace Arch.CMessaging.Client.Net.Core.Session
{
    public interface IoSessionAttributeMap
    {
        Object GetAttribute(IoSession session, Object key, Object defaultValue);
        Object SetAttribute(IoSession session, Object key, Object value);
        Object SetAttributeIfAbsent(IoSession session, Object key, Object value);
        Object RemoveAttribute(IoSession session, Object key);
        Boolean ContainsAttribute(IoSession session, Object key);
        IEnumerable<Object> GetAttributeKeys(IoSession session);
        void Dispose(IoSession session);
    }
}
