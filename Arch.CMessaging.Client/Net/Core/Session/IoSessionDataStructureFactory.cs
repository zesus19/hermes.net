using Arch.CMessaging.Client.Net.Core.Write;

namespace Arch.CMessaging.Client.Net.Core.Session
{
    public interface IoSessionDataStructureFactory
    {
        IoSessionAttributeMap GetAttributeMap(IoSession session);
        IWriteRequestQueue GetWriteRequestQueue(IoSession session);
    }
}
