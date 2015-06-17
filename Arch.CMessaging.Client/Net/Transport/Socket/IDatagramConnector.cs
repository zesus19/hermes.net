using System.Net;
using Arch.CMessaging.Client.Net.Core.Service;

namespace Arch.CMessaging.Client.Net.Transport.Socket
{
    public interface IDatagramConnector : IoConnector
    {
        new IDatagramSessionConfig SessionConfig { get; }
        new IPEndPoint DefaultRemoteEndPoint { get; set; }
    }
}
