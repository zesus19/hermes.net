using System.Net;

namespace Arch.CMessaging.Client.Net.Core.Session
{
    public interface IoSessionRecycler
    {
        void Put(IoSession session);
        IoSession Recycle(EndPoint remoteEP);
        void Remove(IoSession session);
    }

    public class NoopRecycler : IoSessionRecycler
    {
        public static readonly NoopRecycler Instance = new NoopRecycler();

        private NoopRecycler()
        { }

        public void Put(IoSession session)
        {
            // do nothing
        }

        
        public IoSession Recycle(EndPoint remoteEP)
        {
            return null;
        }

        
        public void Remove(IoSession session)
        {
            // do nothing
        }
    }
}
