using System;
using System.Net;
using Arch.CMessaging.Client.Net.Core.Service;

namespace Arch.CMessaging.Client.Net.Transport.Socket
{
    public interface ISocketAcceptor : IoAcceptor
    {
        new ISocketSessionConfig SessionConfig { get; }
        new IPEndPoint LocalEndPoint { get; }
        new IPEndPoint DefaultLocalEndPoint { get; set; }
        Boolean ReuseAddress { get; set; }
        Int32 Backlog { get; set; }
    }
}
