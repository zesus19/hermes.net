using System;

namespace Arch.CMessaging.Client.Net.Filter.KeepAlive
{
    [Serializable]
    public class KeepAliveRequestTimeoutException : Exception
    {
        public KeepAliveRequestTimeoutException() { }
        public KeepAliveRequestTimeoutException(String message) : base(message) { }
        public KeepAliveRequestTimeoutException(String message, Exception inner) : base(message, inner) { }
        protected KeepAliveRequestTimeoutException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
