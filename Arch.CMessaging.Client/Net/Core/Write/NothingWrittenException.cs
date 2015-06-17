using System;

namespace Arch.CMessaging.Client.Net.Core.Write
{
    
    [Serializable]
    public class NothingWrittenException : WriteException
    {
        public NothingWrittenException(IWriteRequest request)
            : base(request)
        { }

        protected NothingWrittenException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }

        public override void GetObjectData(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}
