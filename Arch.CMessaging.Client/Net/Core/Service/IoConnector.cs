using System;
using System.Net;
using Arch.CMessaging.Client.Net.Core.Future;
using Arch.CMessaging.Client.Net.Core.Session;

namespace Arch.CMessaging.Client.Net.Core.Service
{
    public interface IoConnector : IoService
    {
        Int32 ConnectTimeout { get; set; }
        Int64 ConnectTimeoutInMillis { get; set; }
        EndPoint DefaultRemoteEndPoint { get; set; }
        EndPoint DefaultLocalEndPoint { get; set; }
        IConnectFuture Connect();
        IConnectFuture Connect(Action<IoSession, IConnectFuture> sessionInitializer);
        IConnectFuture Connect(EndPoint remoteEP);
        IConnectFuture Connect(EndPoint remoteEP, Action<IoSession, IConnectFuture> sessionInitializer);
        IConnectFuture Connect(EndPoint remoteEP, EndPoint localEP);
        IConnectFuture Connect(EndPoint remoteEP, EndPoint localEP, Action<IoSession, IConnectFuture> sessionInitializer);
    }
}
