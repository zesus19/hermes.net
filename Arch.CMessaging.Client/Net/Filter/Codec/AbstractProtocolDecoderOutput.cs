using System;
using Arch.CMessaging.Client.Net.Core.Filterchain;
using Arch.CMessaging.Client.Net.Core.Session;
using Arch.CMessaging.Client.Net.Util;

namespace Arch.CMessaging.Client.Net.Filter.Codec
{
    public abstract class AbstractProtocolDecoderOutput : IProtocolDecoderOutput
    {
        private readonly IQueue<Object> _queue = new Queue<Object>();

        public IQueue<Object> MessageQueue
        {
            get { return _queue; }
        }

        
        public void Write(Object message)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            _queue.Enqueue(message);
        }

        
        public abstract void Flush(INextFilter nextFilter, IoSession session);
    }
}
