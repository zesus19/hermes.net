using Arch.CMessaging.Client.Net.Core.Session;

namespace Arch.CMessaging.Client.Net.Filter.KeepAlive
{
    public interface IKeepAliveRequestTimeoutHandler
    {
        void KeepAliveRequestTimedOut(KeepAliveFilter filter, IoSession session);
    }
}
