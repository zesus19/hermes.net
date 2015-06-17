using System;
using System.Diagnostics;
namespace Arch.CMessaging.Client.Net.Util
{
    public abstract class ExceptionMonitor
    {
        private static ExceptionMonitor _instance = DefaultExceptionMonitor.Monitor;

        public static ExceptionMonitor Instance
        {
            get { return _instance; }
            set { _instance = value ?? DefaultExceptionMonitor.Monitor; }
        }

        public abstract void ExceptionCaught(Exception cause);
    }

    class DefaultExceptionMonitor : ExceptionMonitor
    {
        public static readonly DefaultExceptionMonitor Monitor = new DefaultExceptionMonitor();

        public override void ExceptionCaught(Exception cause)
        {
            Debug.WriteLine("Unexpected exception.", cause);
        }
    }
}
