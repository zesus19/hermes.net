using System;
using System.Net;
using System.Net.Sockets;
using Arch.CMessaging.Client.Net.Core.Buffer;
using Arch.CMessaging.Client.Net.Core.File;
using Arch.CMessaging.Client.Net.Core.Filterchain;
using Arch.CMessaging.Client.Net.Core.Service;
using Arch.CMessaging.Client.Net.Core.Write;
using Arch.CMessaging.Client.Net.Filter.Ssl;
using Arch.CMessaging.Client.Net.Util;

namespace Arch.CMessaging.Client.Net.Transport.Socket
{
    public class AsyncSocketSession : SocketSession
    {
        public static readonly ITransportMetadata Metadata
            = new DefaultTransportMetadata("async", "socket", false, true, typeof(IPEndPoint));

        private readonly SocketAsyncEventArgsBuffer _readBuffer;
        private readonly SocketAsyncEventArgsBuffer _writeBuffer;
        private readonly EventHandler<SocketAsyncEventArgs> _completeHandler;

        public AsyncSocketSession(IoService service, IoProcessor<SocketSession> processor, System.Net.Sockets.Socket socket,
            SocketAsyncEventArgsBuffer readBuffer, SocketAsyncEventArgsBuffer writeBuffer, Boolean reuseBuffer)
            : base(service, processor, new SessionConfigImpl(socket), socket, socket.LocalEndPoint, socket.RemoteEndPoint, reuseBuffer)
        {
            _readBuffer = readBuffer;
            _readBuffer.SocketAsyncEventArgs.UserToken = this;
            _writeBuffer = writeBuffer;
            _writeBuffer.SocketAsyncEventArgs.UserToken = this;
            _completeHandler = saea_Completed;
        }

        public SocketAsyncEventArgsBuffer ReadBuffer
        {
            get { return _readBuffer; }
        }

        public SocketAsyncEventArgsBuffer WriteBuffer
        {
            get { return _writeBuffer; }
        }

        
        public override ITransportMetadata TransportMetadata
        {
            get { return Metadata; }
        }

        public override Boolean Secured
        {
            get
            {
                IoFilterChain chain = this.FilterChain;
                SslFilter sslFilter = (SslFilter)chain.Get(typeof(SslFilter));
                return sslFilter != null && sslFilter.IsSslStarted(this);
            }
        }
 
        protected override void BeginSend(IWriteRequest request, IoBuffer buf)
        {
            _writeBuffer.Clear();

            SocketAsyncEventArgs saea;
            SocketAsyncEventArgsBuffer saeaBuf = buf as SocketAsyncEventArgsBuffer;
            if (saeaBuf == null)
            {
                if (_writeBuffer.Remaining < buf.Remaining)
                {
                    Int32 oldLimit = buf.Limit;
                    buf.Limit = buf.Position + _writeBuffer.Remaining;
                    _writeBuffer.Put(buf);
                    buf.Limit = oldLimit;
                }
                else
                {
                    _writeBuffer.Put(buf);
                }
                _writeBuffer.Flip();
                saea = _writeBuffer.SocketAsyncEventArgs;
                saea.SetBuffer(saea.Offset + _writeBuffer.Position, _writeBuffer.Limit);
            }
            else
            {
                saea = saeaBuf.SocketAsyncEventArgs;
                saea.Completed += _completeHandler;
            }

            Boolean willRaiseEvent;
            try
            {
                willRaiseEvent = Socket.SendAsync(saea);
            }
            catch (ObjectDisposedException)
            {
                // do nothing
                return;
            }
            catch (Exception ex)
            {
                EndSend(ex);
                return;
            }
            if (!willRaiseEvent)
            {
                ProcessSend(saea);
            }
        }
       
        protected override void BeginSendFile(IWriteRequest request, IFileRegion file)
        {
            SocketAsyncEventArgs saea = _writeBuffer.SocketAsyncEventArgs;
            saea.SendPacketsElements = new SendPacketsElement[] {
                new SendPacketsElement(file.FullName)
            };

            Boolean willRaiseEvent;
            try
            {
                willRaiseEvent = Socket.SendPacketsAsync(saea);
            }
            catch (ObjectDisposedException)
            {
                // do nothing
                return;
            }
            catch (Exception ex)
            {
                EndSend(ex);
                return;
            }
            if (!willRaiseEvent)
            {
                ProcessSend(saea);
            }
        }

        void saea_Completed(Object sender, SocketAsyncEventArgs e)
        {
            e.Completed -= _completeHandler;
            ProcessSend(e);
        }

        public void ProcessSend(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                EndSend(e.BytesTransferred);
            }
            else if (e.SocketError != SocketError.OperationAborted
                && e.SocketError != SocketError.Interrupted
                && e.SocketError != SocketError.ConnectionReset)
            {
                EndSend(new SocketException((Int32)e.SocketError));
            }
            else
            {
                // closed
                Processor.Remove(this);
            }
        }
       
        protected override void BeginReceive()
        {
            _readBuffer.Clear();

            Boolean willRaiseEvent;
            try
            {
                willRaiseEvent = Socket.ReceiveAsync(_readBuffer.SocketAsyncEventArgs);
            }
            catch (ObjectDisposedException)
            {
                // do nothing
                return;
            }
            catch (Exception ex)
            {
                EndReceive(ex);
                return;
            }
            if (!willRaiseEvent)
            {
                ProcessReceive(_readBuffer.SocketAsyncEventArgs);
            }
        }

        public void ProcessReceive(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                if (e.BytesTransferred > 0)
                {
                    _readBuffer.Position = e.BytesTransferred;
                    _readBuffer.Flip();

                    if (ReuseBuffer)
                    {
                        EndReceive(_readBuffer);
                    }
                    else
                    {
                        IoBuffer buf = IoBuffer.Allocate(_readBuffer.Remaining);
                        buf.Put(_readBuffer);
                        buf.Flip();
                        EndReceive(buf);
                    }

                    return;
                }
                else
                {
                    // closed
                    //Processor.Remove(this);
                    this.FilterChain.FireInputClosed();
                }
            }
            else if (e.SocketError != SocketError.OperationAborted
                && e.SocketError != SocketError.Interrupted
                && e.SocketError != SocketError.ConnectionReset)
            {
                EndReceive(new SocketException((Int32)e.SocketError));
            }
            else
            {
                // closed
                Processor.Remove(this);
            }
        }
    }
}
