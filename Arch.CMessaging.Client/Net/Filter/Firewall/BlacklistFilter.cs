using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Arch.CMessaging.Client.Net.Core.Filterchain;
using Arch.CMessaging.Client.Net.Core.Session;
using Arch.CMessaging.Client.Net.Core.Write;
using System.Diagnostics;

namespace Arch.CMessaging.Client.Net.Filter.Firewall
{
    public class BlacklistFilter : IoFilterAdapter
    {
        private readonly List<Subnet> _blacklist = new List<Subnet>();

        public override void SessionCreated(INextFilter nextFilter, IoSession session)
        {
            if (IsBlocked(session))
                BlockSession(session);
            else
                // forward if not blocked
                base.SessionCreated(nextFilter, session);
        }

        public override void SessionOpened(INextFilter nextFilter, IoSession session)
        {
            if (IsBlocked(session))
                BlockSession(session);
            else
                // forward if not blocked
                base.SessionOpened(nextFilter, session);
        }

        public override void SessionClosed(INextFilter nextFilter, IoSession session)
        {
            if (IsBlocked(session))
                BlockSession(session);
            else
                // forward if not blocked
                base.SessionClosed(nextFilter, session);
        }

        public override void SessionIdle(INextFilter nextFilter, IoSession session, IdleStatus status)
        {
            if (IsBlocked(session))
                BlockSession(session);
            else
                // forward if not blocked
                base.SessionIdle(nextFilter, session, status);
        }

        public override void MessageReceived(INextFilter nextFilter, IoSession session, Object message)
        {
            if (IsBlocked(session))
                BlockSession(session);
            else
                // forward if not blocked
                base.MessageReceived(nextFilter, session, message);
        }

        public override void MessageSent(INextFilter nextFilter, IoSession session, IWriteRequest writeRequest)
        {
            if (IsBlocked(session))
                BlockSession(session);
            else
                // forward if not blocked
                base.MessageSent(nextFilter, session, writeRequest);
        }

        public void SetBlacklist(IEnumerable<IPAddress> addresses)
        {
            if (addresses == null)
                throw new ArgumentNullException("addresses");
            lock (((IList)_blacklist).SyncRoot)
            {
                _blacklist.Clear();
                foreach (IPAddress addr in addresses)
                {
                    Block(addr);
                }
            }
        }

        public void SetSubnetBlacklist(Subnet[] subnets)
        {
            if (subnets == null)
                throw new ArgumentNullException("subnets");
            lock (((IList)_blacklist).SyncRoot)
            {
                _blacklist.Clear();
                foreach (Subnet subnet in subnets)
                {
                    Block(subnet);
                }
            }
        }

        public void Block(IPAddress address)
        {
            if (address == null)
                throw new ArgumentNullException("address");
            Block(new Subnet(address, 32));
        }

        public void Block(Subnet subnet)
        {
            if (subnet == null)
                throw new ArgumentNullException("subnet");
            lock (((IList)_blacklist).SyncRoot)
            {
                _blacklist.Add(subnet);
            }
        }

        public void Unblock(IPAddress address)
        {
            if (address == null)
                throw new ArgumentNullException("address");
            Unblock(new Subnet(address, 32));
        }

        private void Unblock(Subnet subnet)
        {
            if (subnet == null)
                throw new ArgumentNullException("subnet");
            lock (((IList)_blacklist).SyncRoot)
            {
                _blacklist.Remove(subnet);
            }
        }

        private void BlockSession(IoSession session)
        {
            Debug.WriteLine("Remote address in the blacklist; closing.");
            session.Close(true);
        }

        private Boolean IsBlocked(IoSession session)
        {
            IPEndPoint ep = session.RemoteEndPoint as IPEndPoint;
            if (ep != null)
            {
                IPAddress address = ep.Address;

                // check all subnets
                lock (((IList)_blacklist).SyncRoot)
                {
                    foreach (Subnet subnet in _blacklist)
                    {
                        if (subnet.InSubnet(address))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
