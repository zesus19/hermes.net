using System;

namespace Arch.CMessaging.Client.Net.Filter.Codec.StateMachine
{
    public abstract class ConsumeToLinearWhitespaceDecodingState : ConsumeToDynamicTerminatorDecodingState
    {
        protected override Boolean IsTerminator(Byte b)
        {
            return (b == ' ') || (b == '\t');
        }
    }
}
