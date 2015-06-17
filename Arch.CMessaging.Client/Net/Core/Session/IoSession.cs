using System;
using System.Net;
using Arch.CMessaging.Client.Net.Core.Filterchain;
using Arch.CMessaging.Client.Net.Core.Future;
using Arch.CMessaging.Client.Net.Core.Service;
using Arch.CMessaging.Client.Net.Core.Write;

namespace Arch.CMessaging.Client.Net.Core.Session
{

    public interface IoSession
    {
        Int64 Id { get; }
        IoSessionConfig Config { get; }   
        IoService Service { get; }
        IoProcessor Processor { get; }
        IoHandler Handler { get; }
        IoFilterChain FilterChain { get; }
        IWriteRequestQueue WriteRequestQueue { get; }
        ITransportMetadata TransportMetadata { get; }
        Boolean Connected { get; }
        Boolean Closing { get; }
        Boolean Secured { get; }
        EndPoint LocalEndPoint { get; }
        EndPoint RemoteEndPoint { get; }
        ICloseFuture CloseFuture { get; }
        IWriteFuture Write(Object message);
        IWriteFuture Write(Object message, EndPoint remoteEP);
        ICloseFuture Close(Boolean rightNow);
        T GetAttribute<T>(Object key);
        Object GetAttribute(Object key);
        Object SetAttribute(Object key, Object value);
        Object SetAttribute(Object key);
        Object SetAttributeIfAbsent(Object key, Object value);
        Object RemoveAttribute(Object key);
        Boolean ContainsAttribute(Object key);
        Boolean WriteSuspended { get; }
        Boolean ReadSuspended { get; }
        void SuspendRead();        
        void SuspendWrite();
        void ResumeRead();
        void ResumeWrite();
        IWriteRequest CurrentWriteRequest { get; set; }
        Int64 ReadBytes { get; }
        Int64 WrittenBytes { get; }
        Int64 ReadMessages { get; }
        Int64 WrittenMessages { get; }
        Double ReadBytesThroughput { get; }
        Double WrittenBytesThroughput { get; }
        Double ReadMessagesThroughput { get; }
        Double WrittenMessagesThroughput { get; }
        DateTime CreationTime { get; }
        DateTime LastIoTime { get; }
        DateTime LastReadTime { get; }
        DateTime LastWriteTime { get; }
        Boolean IsIdle(IdleStatus status);
        Boolean IsReaderIdle { get; }
        Boolean IsWriterIdle { get; }
        Boolean IsBothIdle { get; }
        Int32 GetIdleCount(IdleStatus status);
        Int32 ReaderIdleCount { get; }
        Int32 WriterIdleCount { get; }
        Int32 BothIdleCount { get; }
        DateTime GetLastIdleTime(IdleStatus status);
        DateTime LastReaderIdleTime { get; }
        DateTime LastWriterIdleTime { get; }
        DateTime LastBothIdleTime { get; }
        void UpdateThroughput(DateTime currentTime, Boolean force);
    }

    public class IoSessionEventArgs : EventArgs
    {
        private readonly IoSession _session;
        public IoSessionEventArgs(IoSession session)
        {
            _session = session;
        }
        public IoSession Session
        {
            get { return _session; }
        }
    }

    public class IoSessionIdleEventArgs : IoSessionEventArgs
    {
        private readonly IdleStatus _idleStatus;

        
        
        public IoSessionIdleEventArgs(IoSession session, IdleStatus idleStatus)
            : base(session)
        {
            _idleStatus = idleStatus;
        }

        public IdleStatus IdleStatus
        {
            get { return _idleStatus; }
        }
    }

    public class IoSessionExceptionEventArgs : IoSessionEventArgs
    {
        private readonly Exception _exception;

        public IoSessionExceptionEventArgs(IoSession session, Exception exception)
            : base(session)
        {
            _exception = exception;
        }

        public Exception Exception
        {
            get { return _exception; }
        }
    }

    public class IoSessionMessageEventArgs : IoSessionEventArgs
    {
        private readonly Object _message;

        public IoSessionMessageEventArgs(IoSession session, Object message)
            : base(session)
        {
            _message = message;
        }

        public Object Message
        {
            get { return _message; }
        }
    }
}
