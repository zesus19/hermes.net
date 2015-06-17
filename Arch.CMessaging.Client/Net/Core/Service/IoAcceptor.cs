using System;
using System.Collections.Generic;
using System.Net;

namespace Arch.CMessaging.Client.Net.Core.Service
{

    public interface IoAcceptor : IoService
    {
        Boolean CloseOnDeactivation { get; set; }   
        EndPoint LocalEndPoint { get; }
        IEnumerable<EndPoint> LocalEndPoints { get; }
        EndPoint DefaultLocalEndPoint { get; set; }
        IEnumerable<EndPoint> DefaultLocalEndPoints { get; set; }
        void Bind();
        void Bind(EndPoint localEP);
        void Bind(params EndPoint[] localEndPoints);
        void Bind(IEnumerable<EndPoint> localEndPoints);
        void Unbind();
        void Unbind(EndPoint localEP);
        void Unbind(params EndPoint[] localEndPoints);
        void Unbind(IEnumerable<EndPoint> localEndPoints);
    }
}
