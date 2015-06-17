using System;
using Arch.CMessaging.Client.Net.Core.Session;

namespace Arch.CMessaging.Client.Net.Filter.KeepAlive
{
    public interface IKeepAliveMessageFactory
    {
        Boolean IsRequest(IoSession session, Object message);
        Boolean IsResponse(IoSession session, Object message);
        Object GetRequest(IoSession session);
        Object GetResponse(IoSession session, Object request);
    }
}
