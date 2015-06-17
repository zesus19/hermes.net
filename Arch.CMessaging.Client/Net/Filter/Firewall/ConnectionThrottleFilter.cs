using System;
using System.Collections.Concurrent;
using System.Net;
using Arch.CMessaging.Client.Net.Core.Filterchain;
using Arch.CMessaging.Client.Net.Core.Session;
using System.Diagnostics;

namespace Arch.CMessaging.Client.Net.Filter.Firewall
{
    public class ConnectionThrottleFilter : IoFilterAdapter
    {
        static readonly Int64 DefaultTime = 1000L;
        private Int64 _allowedInterval;
        private readonly ConcurrentDictionary<String, DateTime> _clients = new ConcurrentDictionary<String, DateTime>();
        // TODO expire overtime clients

        public ConnectionThrottleFilter()
            : this(DefaultTime)
        { }

        public ConnectionThrottleFilter(Int64 allowedInterval)
        {
            this._allowedInterval = allowedInterval;
        }

        public Int64 AllowedInterval
        {
            get { return _allowedInterval; }
            set { _allowedInterval = value; }
        }

        
        public override void SessionCreated(INextFilter nextFilter, IoSession session)
        {
            if (!IsConnectionOk(session))
            {
                Debug.WriteLine("Connections coming in too fast; closing.");
                session.Close(true);
            }
            base.SessionCreated(nextFilter, session);
        }

        public Boolean IsConnectionOk(IoSession session)
        {
            IPEndPoint ep = session.RemoteEndPoint as IPEndPoint;
            if (ep != null)
            {
                String addr = ep.Address.ToString();
                DateTime now = DateTime.Now;
                DateTime? lastConnTime = null;

                _clients.AddOrUpdate(addr, now, (k, v) =>
                {
                    Debug.WriteLine("This is not a new client");
                    lastConnTime = v;
                    return now;
                });

                if (lastConnTime.HasValue)
                {
                    // if the interval between now and the last connection is
                    // less than the allowed interval, return false
                    if ((now - lastConnTime.Value).TotalMilliseconds < _allowedInterval)
                    {
                        Debug.WriteLine("Session connection interval too short");
                        return false;
                    }
                }

                return true;
            }

            return false;
        }
    }
}
