using Arch.CMessaging.Client.Net.Core.Write;

namespace Arch.CMessaging.Client.Net.Core.Session
{
    class DefaultIoSessionDataStructureFactory : IoSessionDataStructureFactory
    {
        public IoSessionAttributeMap GetAttributeMap(IoSession session)
        {
            return new DefaultIoSessionAttributeMap();
        }

        public IWriteRequestQueue GetWriteRequestQueue(IoSession session)
        {
            return new DefaultWriteRequestQueue();
        }
    }
}
