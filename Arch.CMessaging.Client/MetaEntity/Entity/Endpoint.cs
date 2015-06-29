using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arch.CMessaging.Client.MetaEntity.Entity
{
    [Serializable]
    public class Endpoint
    {
        public const string LOCAL = "local";
        public const string BROKER = "broker";
        public const string TRANSACTION = "transaction";
        public const string KAFKA = "kafka";
        public Endpoint() { }

        public Endpoint(string id) 
        {
            this.ID = id;
        }

        public string ID { get; set; }
        public string Type { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }

        public bool Equals(Endpoint endpoint)
        {
            if (string.IsNullOrEmpty(this.ID))
                return false;
            if (string.IsNullOrEmpty(endpoint.ID))
                return false;
            return this.ID.Equals(endpoint.ID);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Endpoint && Equals((Endpoint)obj);
        }

        public override int GetHashCode()
        {
            return string.IsNullOrEmpty(this.ID) ? base.GetHashCode() :  this.ID.GetHashCode();
        }
    }
}
