using System.Net;
using Arch.CMessaging.Client.Net.Core.Service;
using Arch.CMessaging.Client.Net.Core.Session;

namespace Arch.CMessaging.Client.Net.Transport.Socket
{
    public interface IDatagramAcceptor : IoAcceptor
    {
        new IDatagramSessionConfig SessionConfig { get; }
        new IPEndPoint LocalEndPoint { get; }
        new IPEndPoint DefaultLocalEndPoint { get; set; }
        IoSessionRecycler SessionRecycler { get; set; }
    }
}
