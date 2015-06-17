using System;
using System.Collections.Generic;

namespace Arch.CMessaging.Client.Net.Core.Write
{
    
    [Serializable]
    public class WriteToClosedSessionException : WriteException
    {
        public WriteToClosedSessionException(IWriteRequest request)
            : base(request)
        { }

        public WriteToClosedSessionException(IEnumerable<IWriteRequest> requests)
            : base(requests)
        { }

        protected WriteToClosedSessionException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
