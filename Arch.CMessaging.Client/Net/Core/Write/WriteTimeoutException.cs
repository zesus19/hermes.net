using System;
using System.Collections.Generic;

namespace Arch.CMessaging.Client.Net.Core.Write
{
    
    [Serializable]
    public class WriteTimeoutException : WriteException
    {
        public WriteTimeoutException(IWriteRequest request)
            : base(request)
        { }
        public WriteTimeoutException(IEnumerable<IWriteRequest> requests)
            : base(requests)
        { }
        protected WriteTimeoutException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
