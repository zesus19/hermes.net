using Arch.CMessaging.Client.Net.Core.Buffer;

namespace Arch.CMessaging.Client.Net.Filter.Codec.StateMachine
{
    public interface IDecodingState
    {
        IDecodingState Decode(IoBuffer input, IProtocolDecoderOutput output);
        IDecodingState FinishDecode(IProtocolDecoderOutput output);
    }
}
