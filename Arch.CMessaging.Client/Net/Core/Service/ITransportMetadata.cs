using System;

namespace Arch.CMessaging.Client.Net.Core.Service
{
    public interface ITransportMetadata
    {
        String ProviderName { get; }
        String Name { get; }
        Boolean Connectionless { get; }
        Boolean HasFragmentation { get; }
        Type EndPointType { get; }
    }
}
