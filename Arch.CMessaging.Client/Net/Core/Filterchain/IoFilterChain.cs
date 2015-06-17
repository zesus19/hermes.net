using System;
using Arch.CMessaging.Client.Net.Core.Session;
using Arch.CMessaging.Client.Net.Core.Write;

namespace Arch.CMessaging.Client.Net.Core.Filterchain
{
    
    public interface IoFilterChain : IChain<IoFilter, INextFilter>
    {
        IoSession Session { get; }
        void FireSessionCreated();
        void FireSessionOpened();
        void FireSessionClosed();
        void FireSessionIdle(IdleStatus status);
        void FireMessageReceived(Object message);
        void FireMessageSent(IWriteRequest request);
        void FireExceptionCaught(Exception ex);
        void FireInputClosed();
        void FireFilterWrite(IWriteRequest writeRequest);
        void FireFilterClose();
    }
}
