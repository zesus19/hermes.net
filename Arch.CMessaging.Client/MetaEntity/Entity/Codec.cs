using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arch.CMessaging.Client.MetaEntity.Entity
{
    [Serializable]
    public class Codec
    {
        public const String JSON = "json";
	    public const String AVRO = "avro";
	    public const String CMESSAGING = "cmessaging";

        public Codec() : this(null) { }

        public Codec(string type)
        {
            this.Type = type;
            this.Properties = new Dictionary<string, Property>();
        }

        public string Type { get; set; }
        public Dictionary<string, Property> Properties { get; private set; }

        public Codec AddProperty(Property property)
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

        public bool Equals(Codec codec)
        {
            if (string.IsNullOrEmpty(this.Type))
                return false;
            if (string.IsNullOrEmpty(codec.Type))
                return false;
            return this.Type.Equals(codec.Type);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Codec && Equals((Codec)obj);
        }

        public override int GetHashCode()
        {
            return string.IsNullOrEmpty(this.Type) ? base.GetHashCode() : this.Type.GetHashCode();
        }
    }
}
