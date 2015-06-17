namespace Arch.CMessaging.Client.Net.Core.Session
{
    public enum IdleStatus
    {
        ReaderIdle,
        WriterIdle,
        BothIdle
    }

    public class IdleEventArgs : System.EventArgs
    {
        private readonly IdleStatus _idleStatus;

        public IdleEventArgs(IdleStatus idleStatus)
        {
            _idleStatus = idleStatus;
        }

        public IdleStatus IdleStatus
        {
            get { return _idleStatus; }
        }
    }
}
