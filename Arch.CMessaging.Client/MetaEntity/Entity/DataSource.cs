using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arch.CMessaging.Client.MetaEntity.Entity
{
    public class DataSource
    {
        public DataSource()
        {
            Properties = new Dictionary<string, Property>();
        }
        public string ID { get; set; }
        public Dictionary<string, Property> Properties { get; private set; }

        public bool Equals(DataSource dataSource)
        {
            if (string.IsNullOrEmpty(this.ID))
                return false;
            return this.ID.Equals(dataSource.ID);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is DataSource && Equals((DataSource)obj);
        }

        public override int GetHashCode()
        {
            return string.IsNullOrEmpty(this.ID) ? base.GetHashCode() : this.ID.GetHashCode();
        }

        public DataSource AddProperty(Property property)
        {
            if (property != null && 
                !string.IsNullOrEmpty(property.Name))
            {
                Properties[property.Name] = property;
            }
            return this;
        }

        public Property FindProperty(string name)
        {
            Property property = null;
            if (!string.IsNullOrEmpty(name))
            {
                Properties.TryGetValue(name, out property);
            }
            return property;
        }

        public bool RemoveProperty(string name)
        {
            if (string.IsNullOrEmpty(name)) return false;
            return Properties.Remove(name);
        }
    }
}
