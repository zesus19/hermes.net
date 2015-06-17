using System;
using Arch.CMessaging.Client.Net.Core.Session;

namespace Arch.CMessaging.Client.Net.Handler.Chain
{
    public interface INextCommand
    {
        void Execute(IoSession session, Object message);
    }
}
