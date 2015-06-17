using System;

namespace Arch.CMessaging.Client.Net.Filter.Codec.StateMachine
{
    public abstract class LinearWhitespaceSkippingState : SkippingState
    {
        protected override Boolean CanSkip(Byte b)
        {
            return b == 32 || b == 9;
        }
    }
}
