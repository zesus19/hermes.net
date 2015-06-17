using System;

namespace Arch.CMessaging.Client.Net.Core.Session
{
    public interface IoSessionConfig
    {
        Int32 ReadBufferSize { get; set; }
        Int32 ThroughputCalculationInterval { get; set; }
        Int64 ThroughputCalculationIntervalInMillis { get; }
        Int32 GetIdleTime(IdleStatus status);
        Int64 GetIdleTimeInMillis(IdleStatus status);
        void SetIdleTime(IdleStatus status, Int32 idleTime);
        Int32 ReaderIdleTime { get; set; }
        Int32 WriterIdleTime { get; set; }
        Int32 BothIdleTime { get; set; }
        Int32 WriteTimeout { get; set; }
        Int64 WriteTimeoutInMillis { get; }
        void SetAll(IoSessionConfig config);
    }
}
