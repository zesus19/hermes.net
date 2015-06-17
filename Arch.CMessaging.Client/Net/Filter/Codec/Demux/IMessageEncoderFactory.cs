namespace Arch.CMessaging.Client.Net.Filter.Codec.Demux
{
    public interface IMessageEncoderFactory
    {
        IMessageEncoder GetEncoder();
    }

    public interface IMessageEncoderFactory<T> : IMessageEncoderFactory
    {
        new IMessageEncoder<T> GetEncoder();
    }
}
