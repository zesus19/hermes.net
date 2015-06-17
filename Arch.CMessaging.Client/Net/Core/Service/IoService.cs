using System;
using System.Collections.Generic;
using Arch.CMessaging.Client.Net.Core.Filterchain;
using Arch.CMessaging.Client.Net.Core.Future;
using Arch.CMessaging.Client.Net.Core.Session;

namespace Arch.CMessaging.Client.Net.Core.Service
{
    public interface IoService : IDisposable
    {
        ITransportMetadata TransportMetadata { get; }
        Boolean Disposed { get; }
        IoHandler Handler { get; set; }
        IDictionary<Int64, IoSession> ManagedSessions { get; }
        Boolean Active { get; }
        DateTime ActivationTime { get; }
        IoSessionConfig SessionConfig { get; }
        IoFilterChainBuilder FilterChainBuilder { get; set; }
        DefaultIoFilterChainBuilder FilterChain { get; }
        IoSessionDataStructureFactory SessionDataStructureFactory { get; set; }
        IEnumerable<IWriteFuture> Broadcast(Object message);
        event EventHandler Activated;
        event EventHandler<IdleEventArgs> Idle;
        event EventHandler Deactivated;
        event EventHandler<IoSessionEventArgs> SessionCreated;
        event EventHandler<IoSessionEventArgs> SessionDestroyed;
        event EventHandler<IoSessionEventArgs> SessionOpened;
        event EventHandler<IoSessionEventArgs> SessionClosed;
        event EventHandler<IoSessionIdleEventArgs> SessionIdle;
        event EventHandler<IoSessionExceptionEventArgs> ExceptionCaught;
        event EventHandler<IoSessionEventArgs> InputClosed;
        event EventHandler<IoSessionMessageEventArgs> MessageReceived;
        event EventHandler<IoSessionMessageEventArgs> MessageSent;
        IoServiceStatistics Statistics { get; }
    }
}
