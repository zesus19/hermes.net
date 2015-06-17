using System;
using Arch.CMessaging.Client.Net.Core.Session;

namespace Arch.CMessaging.Client.Net.Handler.Demux
{
    public interface IExceptionHandler
    {
        void ExceptionCaught(IoSession session, Exception cause);
    }

    public interface IExceptionHandler<in E> : IExceptionHandler where E : Exception
    {
        
        /// Invoked when the specific type of exception is caught from the
        /// specified <code>session</code>.
        
        void ExceptionCaught(IoSession session, E cause);
    }
}
