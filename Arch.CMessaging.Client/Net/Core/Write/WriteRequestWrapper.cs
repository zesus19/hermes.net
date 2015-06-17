using System;
using System.Net;
using Arch.CMessaging.Client.Net.Core.Future;

namespace Arch.CMessaging.Client.Net.Core.Write
{
    public class WriteRequestWrapper : IWriteRequest
    {
        private readonly IWriteRequest _inner;

        
        
        public WriteRequestWrapper(IWriteRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");
            _inner = request;
        }

        
        public IWriteRequest OriginalRequest
        {
            get { return _inner.OriginalRequest; }
        }

        
        public virtual Object Message
        {
            get { return _inner.Message; }
        }

        
        public EndPoint Destination
        {
            get { return _inner.Destination; }
        }

        
        public Boolean Encoded
        {
            get { return _inner.Encoded; }
        }

        
        public IWriteFuture Future
        {
            get { return _inner.Future; }
        }

        public IWriteRequest InnerRequest
        {
            get { return _inner; }
        }
    }
}
