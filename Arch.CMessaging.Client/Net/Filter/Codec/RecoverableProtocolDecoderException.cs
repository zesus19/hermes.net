using System;

namespace Arch.CMessaging.Client.Net.Filter.Codec
{
    [Serializable]
    public class RecoverableProtocolDecoderException : ProtocolDecoderException
    {
        public RecoverableProtocolDecoderException() { }

        public RecoverableProtocolDecoderException(String message)
            : base(message) { }

        public RecoverableProtocolDecoderException(String message, Exception innerException)
            : base(message, innerException) { }

        protected RecoverableProtocolDecoderException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
