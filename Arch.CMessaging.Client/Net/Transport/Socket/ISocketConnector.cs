using System.Net;
using Arch.CMessaging.Client.Net.Core.Service;

namespace Arch.CMessaging.Client.Net.Transport.Socket
{
    public interface ISocketConnector : IoConnector
    {
        new ISocketSessionConfig SessionConfig { get; }
        new IPEndPoint DefaultRemoteEndPoint { get; set; }
    }
}
