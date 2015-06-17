using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Arch.CMessaging.Client.Net.Core.Future;
using Arch.CMessaging.Client.Net.Core.Service;
using Arch.CMessaging.Client.Net.Core.Session;
using Arch.CMessaging.Client.Net.Util;

namespace Arch.CMessaging.Client.Net.Transport.Socket
{

    public abstract class AbstractSocketAcceptor : AbstractIoAcceptor, ISocketAcceptor
    {
        private readonly AsyncSocketProcessor _processor;
        private Int32 _backlog;
        private Int32 _maxConnections;
        private Semaphore _connectionPool;
        private readonly Action<Object> _startAccept;
        private Boolean _disposed;
        private readonly Dictionary<EndPoint, System.Net.Sockets.Socket> _listenSockets = new Dictionary<EndPoint, System.Net.Sockets.Socket>();

        protected AbstractSocketAcceptor()
            : this(1024)
        { }

        protected AbstractSocketAcceptor(Int32 maxConnections)
            : base(new DefaultSocketSessionConfig())
        {
            _maxConnections = maxConnections;
            _processor = new AsyncSocketProcessor(() => ManagedSessions.Values);
            this.SessionDestroyed += OnSessionDestroyed;
            _startAccept = StartAccept0;
            ReuseBuffer = true;
        }

        
        public new ISocketSessionConfig SessionConfig
        {
            get { return (ISocketSessionConfig)base.SessionConfig; }
        }
        
        public new IPEndPoint LocalEndPoint
        {
            get { return (IPEndPoint)base.LocalEndPoint; }
        }
        
        public new IPEndPoint DefaultLocalEndPoint
        {
            get { return (IPEndPoint)base.DefaultLocalEndPoint; }
            set { base.DefaultLocalEndPoint = value; }
        }
       
        public override ITransportMetadata TransportMetadata
        {
            get { return AsyncSocketSession.Metadata; }
        }
        
        public Boolean ReuseAddress { get; set; }
        
        public Int32 Backlog
        {
            get { return _backlog; }
            set
            {
                lock (_bindLock)
                {
                    if (Active)
                        throw new InvalidOperationException("Backlog can't be set while the acceptor is bound.");
                    _backlog = value;
                }
            }
        }

        public Int32 MaxConnections
        {
            get { return _maxConnections; }
            set
            {
                lock (_bindLock)
                {
                    if (Active)
                        throw new InvalidOperationException("MaxConnections can't be set while the acceptor is bound.");
                    _maxConnections = value;
                }
            }
        }

        public Boolean ReuseBuffer { get; set; }

        protected override IEnumerable<EndPoint> BindInternal(IEnumerable<EndPoint> localEndPoints)
        {
            Dictionary<EndPoint, System.Net.Sockets.Socket> newListeners = new Dictionary<EndPoint, System.Net.Sockets.Socket>();
            try
            {
                // Process all the addresses
                foreach (EndPoint localEP in localEndPoints)
                {
                    EndPoint ep = localEP;
                    if (ep == null)
                        ep = new IPEndPoint(IPAddress.Any, 0);
                    System.Net.Sockets.Socket listenSocket = new System.Net.Sockets.Socket(ep.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    listenSocket.Bind(ep);
                    listenSocket.Listen(Backlog);
                    newListeners[listenSocket.LocalEndPoint] = listenSocket;
                }
            }
            catch (Exception)
            {
                // Roll back if failed to bind all addresses
                foreach (System.Net.Sockets.Socket listenSocket in newListeners.Values)
                {
                    try
                    {
                        listenSocket.Close();
                    }
                    catch (Exception ex)
                    {
                        ExceptionMonitor.Instance.ExceptionCaught(ex);
                    }
                }

                throw;
            }

            if (MaxConnections > 0)
                _connectionPool = new Semaphore(MaxConnections, MaxConnections);

            foreach (KeyValuePair<EndPoint, System.Net.Sockets.Socket> pair in newListeners)
            {
                _listenSockets[pair.Key] = pair.Value;
                StartAccept(new ListenerContext(pair.Value));
            }

            _processor.IdleStatusChecker.Start();

            return newListeners.Keys;
        }
        
        protected override void UnbindInternal(IEnumerable<EndPoint> localEndPoints)
        {
            foreach (EndPoint ep in localEndPoints)
            {
                System.Net.Sockets.Socket listenSocket;
                if (!_listenSockets.TryGetValue(ep, out listenSocket))
                    continue;
                listenSocket.Close();
                _listenSockets.Remove(ep);
            }

            if (_listenSockets.Count == 0)
            {
                _processor.IdleStatusChecker.Stop();

                if (_connectionPool != null)
                {
                    _connectionPool.Close();
                    _connectionPool = null;
                }
            }
        }

        private void StartAccept(ListenerContext listener)
        {
            if (_connectionPool == null)
            {
                BeginAccept(listener);
            }
            else
            {
                System.Threading.Tasks.Task.Factory.StartNew(_startAccept, listener);
            }
        }

        private void StartAccept0(Object state)
        {
            Semaphore pool = _connectionPool;
            if (pool == null)
                // this might happen if has been unbound
                return;
            try
            {
                pool.WaitOne();
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            BeginAccept((ListenerContext)state);
        }

        private void OnSessionDestroyed(Object sender, IoSessionEventArgs e)
        {
            Semaphore pool = _connectionPool;
            if (pool != null)
                pool.Release();
        }
        protected abstract IoSession NewSession(IoProcessor<SocketSession> processor, System.Net.Sockets.Socket socket);
        protected abstract void BeginAccept(ListenerContext listener);
        protected void EndAccept(System.Net.Sockets.Socket socket, ListenerContext listener)
        {
            if (socket != null)
            {
                IoSession session = NewSession(_processor, socket);
                try
                {
                    InitSession<IoFuture>(session, null, null);
                    session.Processor.Add(session);
                }
                catch (Exception ex)
                {
                    ExceptionMonitor.Instance.ExceptionCaught(ex);
                }
            }

            // Accept the next connection request
            StartAccept(listener);
        }
       
        protected override void Dispose(Boolean disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_listenSockets.Count > 0)
                    {
                        foreach (System.Net.Sockets.Socket listenSocket in _listenSockets.Values)
                        {
                            ((IDisposable)listenSocket).Dispose();
                        }
                    }
                    if (_connectionPool != null)
                    {
                        ((IDisposable)_connectionPool).Dispose();
                        _connectionPool = null;
                    }
                    _processor.Dispose();
                }
                _disposed = true;
            }
            base.Dispose(disposing);
        }
   
        protected class ListenerContext
        {
            private readonly System.Net.Sockets.Socket _socket;
            public ListenerContext(System.Net.Sockets.Socket socket)
            {
                _socket = socket;
            }
            public System.Net.Sockets.Socket Socket
            {
                get { return _socket; }
            }
            public Object Tag { get; set; }
        }
    }
}
