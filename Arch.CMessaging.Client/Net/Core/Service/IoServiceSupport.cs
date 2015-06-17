using Arch.CMessaging.Client.Net.Core.Session;

namespace Arch.CMessaging.Client.Net.Core.Service
{
    interface IoServiceSupport
    {
        void FireServiceActivated();
        void FireServiceIdle(IdleStatus idleStatus);
        void FireSessionCreated(IoSession session);
        void FireSessionDestroyed(IoSession session);
        void FireServiceDeactivated();
    }
}
