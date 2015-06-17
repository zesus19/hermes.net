using System;

namespace Arch.CMessaging.Client.Net.Core.Session
{

    [Serializable]
    public sealed class AttributeKey
    {
        private readonly String _name;

        public AttributeKey(Type source, String name)
        {
            _name = source.Name + "." + name + "@" + base.GetHashCode().ToString("X");
        }

        
        public override String ToString()
        {
            return _name;
        }

        
        public override Int32 GetHashCode()
        {
            int h = 17 * 37 + ((_name == null) ? 0 : _name.GetHashCode());
            return h;
        }

        
        public override Boolean Equals(Object obj)
        {
            if (Object.ReferenceEquals(this, obj))
                return true;
            AttributeKey other = obj as AttributeKey;
            if (other == null)
                return false;
            return _name.Equals(other._name);
        }
    }
}
