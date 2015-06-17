using System;
using Arch.CMessaging.Client.Net.Core.Session;

namespace Arch.CMessaging.Client.Net.Transport.Socket
{
    public interface ISocketSessionConfig : IoSessionConfig
    {
        Boolean? ExclusiveAddressUse { get; set; }
        Boolean? ReuseAddress { get; set; }
        Int32? ReceiveBufferSize { get; set; }
        Int32? SendBufferSize { get; set; } 
        Int32? TrafficClass { get; set; }
        Boolean? KeepAlive { get; set; }
        Boolean? OobInline { get; set; }
        Boolean? NoDelay { get; set; }
        Int32? SoLinger { get; set; }
    }
}
