using System;
using System.Collections.Generic;
using Arch.CMessaging.Client.Net.Core.Filterchain;
using Arch.CMessaging.Client.Net.Core.Session;

namespace Arch.CMessaging.Client.Net.Filter.Util
{
    public class SessionAttributeInitializingFilter : IoFilterAdapter
    {
        private readonly Dictionary<String, Object> _attributes = new Dictionary<String, Object>();

        public SessionAttributeInitializingFilter()
        { }

        public SessionAttributeInitializingFilter(IDictionary<String, Object> attributes)
        {
            Attributes = attributes;
        }

        public IDictionary<String, Object> Attributes
        {
            get { return _attributes; }
            set
            {
                _attributes.Clear();
                if (value != null)
                {
                    foreach (KeyValuePair<String, Object> pair in value)
                    {
                        _attributes[pair.Key] = pair.Value;
                    }
                }
            }
        }

        public Object GetAttribute(String key)
        {
            Object value;
            return _attributes.TryGetValue(key, out value) ? value : null;
        }

        public Object SetAttribute(String key, Object value)
        {
            if (value == null)
                return RemoveAttribute(key);

            Object old;
            _attributes.TryGetValue(key, out old);
            _attributes[key] = value;
            return old;
        }

        public Object SetAttribute(String key)
        {
            return SetAttribute(key, true);
        }

        public Object RemoveAttribute(String key)
        {
            Object old;
            _attributes.TryGetValue(key, out old);
            _attributes.Remove(key);
            return old;
        }

        public Boolean ContainsAttribute(String key)
        {
            return _attributes.ContainsKey(key);
        }

        public override void SessionCreated(INextFilter nextFilter, IoSession session)
        {
            foreach (KeyValuePair<String, Object> pair in _attributes)
            {
                session.SetAttribute(pair.Key, pair.Value);
            }

            base.SessionCreated(nextFilter, session);
        }
    }
}
