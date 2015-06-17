using System;
using System.Net;
using Arch.CMessaging.Client.Net.Core.Future;

namespace Arch.CMessaging.Client.Net.Core.Write
{
    public interface IWriteRequest
    {
        
        IWriteRequest OriginalRequest { get; }
        Object Message { get; }
        EndPoint Destination { get; }
        Boolean Encoded { get; }
        IWriteFuture Future { get; }
    }
}
