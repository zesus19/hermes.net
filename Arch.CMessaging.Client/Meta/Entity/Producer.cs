using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arch.CMessaging.Client.Meta.Entity
{
    public class Producer
    {
        public long AppID { get; set; }
        public bool Equals(Producer producer)
        {
            return this.AppID == producer.AppID;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Producer && Equals((Producer)obj);
        }

        public override int GetHashCode()
        {
            return this.AppID.GetHashCode();
        }
    }
}
