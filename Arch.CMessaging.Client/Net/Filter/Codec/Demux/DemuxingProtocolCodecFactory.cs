using System;
using Arch.CMessaging.Client.Net.Core.Session;

namespace Arch.CMessaging.Client.Net.Filter.Codec.Demux
{
    public class DemuxingProtocolCodecFactory : IProtocolCodecFactory
    {
        internal static readonly Type[] EmptyParams = new Type[0];
        private readonly DemuxingProtocolEncoder encoder = new DemuxingProtocolEncoder();
        private readonly DemuxingProtocolDecoder decoder = new DemuxingProtocolDecoder();
        
        public IProtocolEncoder GetEncoder(IoSession session)
        {
            return encoder;
        }

        
        public IProtocolDecoder GetDecoder(IoSession session)
        {
            return decoder;
        }

        public void AddMessageEncoder<TMessage, TEncoder>() where TEncoder : IMessageEncoder
        {
            this.encoder.AddMessageEncoder<TMessage, TEncoder>();
        }

        public void AddMessageEncoder<TMessage>(IMessageEncoder<TMessage> encoder)
        {
            this.encoder.AddMessageEncoder<TMessage>(encoder);
        }

        public void AddMessageEncoder<TMessage>(IMessageEncoderFactory<TMessage> factory)
        {
            this.encoder.AddMessageEncoder<TMessage>(factory);
        }

        public void AddMessageDecoder<TDecoder>() where TDecoder : IMessageDecoder
        {
            this.decoder.AddMessageDecoder<TDecoder>();
        }

        public void AddMessageDecoder(IMessageDecoder decoder)
        {
            this.decoder.AddMessageDecoder(decoder);
        }

        public void AddMessageDecoder(IMessageDecoderFactory factory)
        {
            this.decoder.AddMessageDecoder(factory);
        }
    }
}
