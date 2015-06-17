using System;

namespace Arch.CMessaging.Client.Net.Filter.Codec
{
    [Serializable]
    public class ProtocolCodecException : Exception
    {
        public ProtocolCodecException()
        { }

        public ProtocolCodecException(String message)
            : base(message)
        { }
        public ProtocolCodecException(String message, Exception innerException)
            : base(message, innerException)
        { }
        protected ProtocolCodecException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class ProtocolEncoderException : ProtocolCodecException
    {
        public ProtocolEncoderException()
        { }
        public ProtocolEncoderException(String message)
            : base(message)
        { }

        public ProtocolEncoderException(String message, Exception innerException)
            : base(message, innerException)
        { }

        protected ProtocolEncoderException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class ProtocolDecoderException : ProtocolCodecException
    {
        private String _hexdump;

        public ProtocolDecoderException()
        { }

        public ProtocolDecoderException(String message)
            : base(message)
        { }

        public ProtocolDecoderException(String message, Exception innerException)
            : base(message, innerException)
        { }

        protected ProtocolDecoderException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }

        public String Hexdump
        {
            get { return _hexdump; }
            set
            {
                if (_hexdump != null)
                    throw new InvalidOperationException("Hexdump cannot be set more than once.");
                _hexdump = value;
            }
        }

        public override void GetObjectData(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}
