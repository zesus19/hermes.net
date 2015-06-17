using System;
using Arch.CMessaging.Client.Net.Core.Future;

namespace Arch.CMessaging.Client.Net.Filter.Codec
{
    public interface IProtocolEncoderOutput
    {
        void Write(Object encodedMessage);
        void MergeAll();
        IWriteFuture Flush();
    }
}
