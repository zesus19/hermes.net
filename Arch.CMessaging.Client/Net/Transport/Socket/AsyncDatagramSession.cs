using System;
using System.Net;
using System.Threading;
using Arch.CMessaging.Client.Net.Core.Buffer;
using Arch.CMessaging.Client.Net.Core.File;
using Arch.CMessaging.Client.Net.Core.Service;
using Arch.CMessaging.Client.Net.Core.Write;

namespace Arch.CMessaging.Client.Net.Transport.Socket
{
    public partial class AsyncDatagramSession : SocketSession
    {
        public static readonly ITransportMetadata Metadata
            = new DefaultTransportMetadata("async", "datagram", true, false, typeof(IPEndPoint));

        private readonly AsyncDatagramAcceptor.SocketContext _socketContext;
        private Int32 _scheduledForFlush;

        internal AsyncDatagramSession(IoService service, IoProcessor<AsyncDatagramSession> processor,
            AsyncDatagramAcceptor.SocketContext ctx, EndPoint remoteEP, Boolean reuseBuffer)
            : base(service, processor, new DefaultDatagramSessionConfig(), ctx.Socket, ctx.Socket.LocalEndPoint, remoteEP, reuseBuffer)
        {
            _socketContext = ctx;
        }

        
        public override ITransportMetadata TransportMetadata
        {
            get { return Metadata; }
        }

        internal AsyncDatagramAcceptor.SocketContext Context
        {
            get { return _socketContext; }
        }

        public Boolean IsScheduledForFlush
        {
            get { return _scheduledForFlush != 0; }
        }

        public Boolean ScheduledForFlush()
        {
            return Interlocked.CompareExchange(ref _scheduledForFlush, 1, 0) == 0;
        }

        public void UnscheduledForFlush()
        {
            Interlocked.Exchange(ref _scheduledForFlush, 0);
        }

        
        protected override void BeginSend(IWriteRequest request, IoBuffer buf)
        {
            EndPoint destination = request.Destination;
            if (destination == null)
                destination = this.RemoteEndPoint;
            BeginSend(buf, destination);
        }

        
        protected override void BeginSendFile(IWriteRequest request, IFileRegion file)
        {
            EndSend(new InvalidOperationException("Cannot send a file via UDP"));
        }
    }
}
