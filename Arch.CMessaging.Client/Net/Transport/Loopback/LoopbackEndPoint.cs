using System;
using System.Net;

namespace Arch.CMessaging.Client.Net.Transport.Loopback
{
    public class LoopbackEndPoint : EndPoint, IComparable<LoopbackEndPoint>
    {
        private readonly Int32 _port;

        public LoopbackEndPoint(Int32 port)
        {
            _port = port;
        }

        public Int32 Port
        {
            get { return _port; }
        }

        
        public Int32 CompareTo(LoopbackEndPoint other)
        {
            return this._port.CompareTo(other._port);
        }

        
        public override Int32 GetHashCode()
        {
            return _port.GetHashCode();
        }

        
        public override Boolean Equals(Object obj)
        {
            if (obj == null)
                return false;
            if (Object.ReferenceEquals(this, obj))
                return true;
            LoopbackEndPoint other = obj as LoopbackEndPoint;
            return obj != null && this._port == other._port;
        }

        
        public override String ToString()
        {
            return _port >= 0 ? ("vm:server:" + _port) : ("vm:client:" + -_port);
        }
    }
}
