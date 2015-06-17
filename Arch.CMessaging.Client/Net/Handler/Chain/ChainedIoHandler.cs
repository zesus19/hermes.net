using System;
using Arch.CMessaging.Client.Net.Core.Service;
using Arch.CMessaging.Client.Net.Core.Session;

namespace Arch.CMessaging.Client.Net.Handler.Chain
{
    public class ChainedIoHandler : IoHandlerAdapter
    {
        private readonly IoHandlerChain _chain;

        public ChainedIoHandler()
            : this(new IoHandlerChain())
        { }

        public ChainedIoHandler(IoHandlerChain chain)
        {
            if (chain == null)
                throw new ArgumentNullException("chain");
            _chain = chain;
        }

        public IoHandlerChain Chain
        {
            get { return _chain; }
        }
 
        public override void MessageReceived(IoSession session, Object message)
        {
            _chain.Execute(null, session, message);
        }
    }
}
