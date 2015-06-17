namespace Arch.CMessaging.Client.Net.Filter.Codec.Demux
{

    public interface IMessageDecoderFactory
    {
        IMessageDecoder GetDecoder();
    }
}
