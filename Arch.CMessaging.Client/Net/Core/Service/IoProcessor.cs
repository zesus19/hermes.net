using Arch.CMessaging.Client.Net.Core.Session;
using Arch.CMessaging.Client.Net.Core.Write;

namespace Arch.CMessaging.Client.Net.Core.Service
{
    public interface IoProcessor
    {
        void Add(IoSession session);
        void Write(IoSession session, IWriteRequest writeRequest);
        void Flush(IoSession session);
        void Remove(IoSession session);
        void UpdateTrafficControl(IoSession session);
    }

    public interface IoProcessor<in S> : IoProcessor
        where S : IoSession
    {
        void Add(S session);
        void Write(S session, IWriteRequest writeRequest);
        void Flush(S session);
        void Remove(S session);
        void UpdateTrafficControl(S session);
    }
}
