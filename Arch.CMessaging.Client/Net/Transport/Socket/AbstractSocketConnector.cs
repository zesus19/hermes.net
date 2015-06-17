using System;
using System.Net;
using System.Net.Sockets;
using Arch.CMessaging.Client.Net.Core.Future;
using Arch.CMessaging.Client.Net.Core.Service;
using Arch.CMessaging.Client.Net.Core.Session;
using Arch.CMessaging.Client.Net.Util;

namespace Arch.CMessaging.Client.Net.Transport.Socket
{
    public abstract class AbstractSocketConnector : AbstractIoConnector
    {
        private readonly AsyncSocketProcessor _processor;

        protected AbstractSocketConnector(IoSessionConfig sessionConfig)
            : base(sessionConfig)
        {
            _processor = new AsyncSocketProcessor(() => ManagedSessions.Values);
        }

        public new IPEndPoint DefaultRemoteEndPoint
        {
            get { return (IPEndPoint)base.DefaultRemoteEndPoint; }
            set { base.DefaultRemoteEndPoint = value; }
        }

        public Boolean ReuseBuffer { get; set; }

        protected IoProcessor<SocketSession> Processor
        {
            get { return _processor; }
        }
       
        protected override IConnectFuture Connect0(EndPoint remoteEP, EndPoint localEP, Action<IoSession, IConnectFuture> sessionInitializer)
        {
            System.Net.Sockets.Socket socket = NewSocket(remoteEP.AddressFamily);
            if (localEP != null)
                socket.Bind(localEP);
            ConnectorContext ctx = new ConnectorContext(socket, remoteEP, sessionInitializer);
            BeginConnect(ctx);
            return ctx.Future;
        }
        protected abstract System.Net.Sockets.Socket NewSocket(AddressFamily addressFamily);
        protected abstract void BeginConnect(ConnectorContext connector);
        protected void EndConnect(IoSession session, ConnectorContext connector)
        {
            try
            {
                InitSession(session, connector.Future, connector.SessionInitializer);
                session.Processor.Add(session);
            }
            catch (Exception ex)
            {
                ExceptionMonitor.Instance.ExceptionCaught(ex);
            }

            _processor.IdleStatusChecker.Start();
        }

        protected void EndConnect(Exception cause, ConnectorContext connector)
        {
            connector.Future.Exception = cause;
            connector.Socket.Close();
        }
       
        protected override void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                _processor.Dispose();
            }
            base.Dispose(disposing);
        }

        protected class ConnectorContext : IDisposable
        {
            private readonly System.Net.Sockets.Socket _socket;
            private readonly EndPoint _remoteEP;
            private readonly Action<IoSession, IConnectFuture> _sessionInitializer;
            private readonly DefaultConnectFuture _future = new DefaultConnectFuture();
            public ConnectorContext(System.Net.Sockets.Socket socket, EndPoint remoteEP, Action<IoSession, IConnectFuture> sessionInitializer)
            {
                _socket = socket;
                _remoteEP = remoteEP;
                _sessionInitializer = sessionInitializer;
            }
            
            public System.Net.Sockets.Socket Socket
            {
                get { return _socket; }
            }

            public EndPoint RemoteEP
            {
                get { return _remoteEP; }
            }

            public IConnectFuture Future
            {
                get { return _future; }
            }

            public Action<IoSession, IConnectFuture> SessionInitializer
            {
                get { return _sessionInitializer; }
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
         
            protected virtual void Dispose(Boolean disposing)
            {
                if (disposing)
                {
                    ((IDisposable)_socket).Dispose();
                    _future.Dispose();
                }
            }
        }
    }
}
