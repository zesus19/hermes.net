using System;
using Arch.CMessaging.Client.Net.Core.Session;

namespace Arch.CMessaging.Client.Net.Transport.Socket
{
    public interface IDatagramSessionConfig : IoSessionConfig
    {
        Boolean? EnableBroadcast { get; set; }
        Boolean? ExclusiveAddressUse { get; set; }
        Boolean? ReuseAddress { get; set; }
        Int32? ReceiveBufferSize { get; set; }
        Int32? SendBufferSize { get; set; }
        Int32? TrafficClass { get; set; }
        System.Net.Sockets.MulticastOption MulticastOption { get; set; }
    }
}
