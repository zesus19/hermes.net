using System;
using Arch.CMessaging.Client.Net.Core.Session;

namespace Arch.CMessaging.Client.Net.Handler.Chain
{
    public interface IoHandlerCommand
    {
        void Execute(INextCommand next, IoSession session, Object message);
    }
}
