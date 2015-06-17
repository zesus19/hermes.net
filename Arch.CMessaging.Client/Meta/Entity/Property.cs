using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arch.CMessaging.Client.Meta.Entity
{
    [Serializable]
    public class Property
    {
        public Property() { }
        public Property(string name)
        {
            this.Name = name;
        }
        public string Name { get; set; }
        public string Value { get; set; }

        public bool Equals(Property property)
        {
            if (string.IsNullOrEmpty(this.Name))
                return false;
            if (string.IsNullOrEmpty(property.Name))
                return false;
            return this.Name.Equals(property.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Property && Equals((Property)obj);
        }

        public override int GetHashCode()
        {
            return string.IsNullOrEmpty(Name) ? base.GetHashCode() : Name.GetHashCode();
        }
    }
}
