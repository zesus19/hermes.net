using System;
using Arch.CMessaging.Client.Net.Core.Session;

namespace Arch.CMessaging.Client.Net.Handler.Demux
{
    public interface IMessageHandler
    {
        
        /// Invoked when the specific type of message is received from or sent to
        /// the specified <code>session</code>.
        
        /// <param name="session">the associated <see cref="IoSession"/></param>
        /// <param name="message">the message to decode</param>
        void HandleMessage(IoSession session, Object message);
    }

    public interface IMessageHandler<in T> : IMessageHandler
    {
        
        /// Invoked when the specific type of message is received from or sent to
        /// the specified <code>session</code>.
        
        /// <param name="session">the associated <see cref="IoSession"/></param>
        /// <param name="message">the message to decode. Its type is set by the implementation</param>
        void HandleMessage(IoSession session, T message);
    }
}
