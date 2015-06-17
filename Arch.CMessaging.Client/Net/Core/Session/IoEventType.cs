using System;

namespace Arch.CMessaging.Client.Net.Core.Session
{
    [Flags]
    public enum IoEventType
    {
        None = 0,
        SessionCreated = 0x1,
        SessionOpened = 0x2,
        SessionClosed = 0x4,
        MessageReceived = 0x8,
        MessageSent = 0x10,
        SessionIdle = 0x20,
        ExceptionCaught = 0x40,
        Write = 0x80,
        Close = 0x100
    }
}
