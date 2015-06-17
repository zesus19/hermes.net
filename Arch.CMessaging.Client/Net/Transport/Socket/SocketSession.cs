using System;
using System.IO;
using System.Net;
using System.Threading;
using Arch.CMessaging.Client.Net.Core.Buffer;
using Arch.CMessaging.Client.Net.Core.File;
using Arch.CMessaging.Client.Net.Core.Filterchain;
using Arch.CMessaging.Client.Net.Core.Service;
using Arch.CMessaging.Client.Net.Core.Session;
using Arch.CMessaging.Client.Net.Core.Write;
using Arch.CMessaging.Client.Net.Util;

namespace Arch.CMessaging.Client.Net.Transport.Socket
{
    public abstract class SocketSession : AbstractIoSession
    {
        private static readonly Object dummy = IoBuffer.Wrap(new Byte[0]);
        private readonly System.Net.Sockets.Socket _socket;
        private readonly EndPoint _localEP;
        private readonly EndPoint _remoteEP;
        private readonly IoProcessor _processor;
        private readonly IoFilterChain _filterChain;
        private Int32 _writing;
        private Object _pendingReceivedMessage = dummy;

        protected SocketSession(IoService service, IoProcessor processor, IoSessionConfig config,
            System.Net.Sockets.Socket socket, EndPoint localEP, EndPoint remoteEP, Boolean reuseBuffer)
            : base(service)
        {
            _socket = socket;
            _localEP = localEP;
            _remoteEP = remoteEP;
            Config = config;
            if (service.SessionConfig != null)
                Config.SetAll(service.SessionConfig);
            _processor = processor;
            _filterChain = new DefaultIoFilterChain(this);
        }
       
        public override IoProcessor Processor
        {
            get { return _processor; }
        }
        
        public override IoFilterChain FilterChain
        {
            get { return _filterChain; }
        }
        
        public override EndPoint LocalEndPoint
        {
            get { return _localEP; }
        }
        
        public override EndPoint RemoteEndPoint
        {
            get { return _remoteEP; }
        }
        
        public System.Net.Sockets.Socket Socket
        {
            get { return _socket; }
        }

        public Boolean ReuseBuffer { get; set; }

        public void Start()
        {
            if (ReadSuspended)
                return;

            if (_pendingReceivedMessage != null)
            {
                if (!Object.ReferenceEquals(_pendingReceivedMessage, dummy))
                    FilterChain.FireMessageReceived(_pendingReceivedMessage);
                _pendingReceivedMessage = null;
                BeginReceive();
            }
        }

        public void Flush()
        {
            if (WriteSuspended)
                return;
            if (Interlocked.CompareExchange(ref _writing, 1, 0) > 0)
                return;
            BeginSend();
        }

        private void BeginSend()
        {
            IWriteRequest req = CurrentWriteRequest;
            if (req == null)
            {
                req = WriteRequestQueue.Poll(this);

                if (req == null)
                {
                    Interlocked.Exchange(ref _writing, 0);
                    return;
                }
                
                CurrentWriteRequest = req;
            }

            IoBuffer buf = req.Message as IoBuffer;

            if (buf == null)
            {
                IFileRegion file = req.Message as IFileRegion;
                if (file == null)
                    EndSend(new InvalidOperationException("Don't know how to handle message of type '"
                            + req.Message.GetType().Name + "'.  Are you missing a protocol encoder?"),
                            true);
                else
                    BeginSendFile(req, file);
            }
            else if (buf.HasRemaining)
            {
                BeginSend(req, buf);
            }
            else
            {
                EndSend(0);
            }
        }

        protected abstract void BeginSend(IWriteRequest request, IoBuffer buf);
        protected abstract void BeginSendFile(IWriteRequest request, IFileRegion file);
        protected void EndSend(Int32 bytesTransferred)
        {
            this.IncreaseWrittenBytes(bytesTransferred, DateTime.Now);

            IWriteRequest req = CurrentWriteRequest;
            if (req != null)
            {
                IoBuffer buf = req.Message as IoBuffer;
                if (buf == null)
                {
                    IFileRegion file = req.Message as IFileRegion;
                    if (file != null)
                    {
                        FireMessageSent(req);
                    }
                    else
                    {
                        // we only send buffers and files so technically it shouldn't happen
                    }
                }
                else if (!buf.HasRemaining)
                {
                    // Buffer has been sent, clear the current request.
                    Int32 pos = buf.Position;
                    buf.Reset();

                    FireMessageSent(req);

                    // And set it back to its position
                    buf.Position = pos;

                    buf.Free();
                }
            }

            if (Socket.Connected)
                BeginSend();
        }

        protected void EndSend(Exception ex)
        {
            EndSend(ex, false);
        }

        protected void EndSend(Exception ex, Boolean discardWriteRequest)
        {
            IWriteRequest req = CurrentWriteRequest;
            if (req != null)
            {
                req.Future.Exception = ex;
                if (discardWriteRequest)
                {
                    CurrentWriteRequest = null;
                    IoBuffer buf = req.Message as IoBuffer;
                    if (buf != null)
                        buf.Free();
                }
            }
            this.FilterChain.FireExceptionCaught(ex);
            if (Socket.Connected)
                BeginSend();
        }

        protected abstract void BeginReceive();
        protected void EndReceive(IoBuffer buf)
        {
            if (ReadSuspended)
            {
                _pendingReceivedMessage = buf;
            }
            else
            {
                FilterChain.FireMessageReceived(buf);

                if (Socket.Connected)
                    BeginReceive();
            }
        }

        protected void EndReceive(Exception ex)
        {
            this.FilterChain.FireExceptionCaught(ex);
            if (Socket.Connected && !ReadSuspended)
                BeginReceive();
        }

        private void FireMessageSent(IWriteRequest req)
        {
            CurrentWriteRequest = null;
            try
            {
                this.FilterChain.FireMessageSent(req);
            }
            catch (Exception ex)
            {
                this.FilterChain.FireExceptionCaught(ex);
            }
        }
    }
}
