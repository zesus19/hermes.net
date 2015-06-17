using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arch.CMessaging.Client.Meta.Entity
{
    [Serializable]
    public class Partition
    {
        public int ID { get; set; }
        public string ReadDataSource { get; set; }
        public string WriteDataSource { get; set; }
        public string Endpoint { get; set; }

        public bool Equals(Partition partition)
        {
            return this.ID.Equals(partition.ID);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Partition && Equals((Partition)obj);
        }

        public override int GetHashCode()
        {
            return this.ID.GetHashCode();
        }
    }
}
