using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arch.CMessaging.Client.Meta.Entity
{
    [Serializable]
    public class Storage
    {
        public const string MEMORY = "memory";
        public const string KAFKA = "kafka";
        public const string MYSQL = "mysql";

        public string Type { get; set; }
        public bool Default { get; set; }
        public List<Property> Properties { get; private set; }
        public List<DataSource> DataSources { get; private set; }
        public Dictionary<int, Partition> Partitions { get; private set; }

        public Storage() : this(null) { }

        public Storage(string type)
        {
            this.Type = type;
            this.Properties = new List<Property>();
            this.Partitions = new Dictionary<int, Partition>();
        }

        public Storage AddDataSource(DataSource dataSource)
        {
            if (dataSource != null)
                DataSources.Add(dataSource);
            return this;
        }

        public Storage AddPartition(Partition partition)
        {
            if (partition != null)
                Partitions[partition.ID] = partition;
            return this;
        }

        public Storage AddProperty(Property property)
        {
            if (property != null)
                Properties.Add(property);
            return this;
        }

        public Partition FindPartition(int id)
        {
            Partition partition = null;
            Partitions.TryGetValue(id, out partition);
            return partition;
        }

        public bool RemovePartition(int id)
        {
            return Partitions.Remove(id);
        }

        public Property FindProperty(string name)
        {
            Property property = null;
            if (!string.IsNullOrEmpty(name))
            {
                property = Properties.Find(p => name.Equals(p.Name));
            }
            return property;
        }

        public bool RemoveProperty(string name)
        {
            if (string.IsNullOrEmpty(name)) return false;
            return Properties.RemoveAll(p => name.Equals(p.Name)) > 0;
        }

        public bool Equals(Storage storage)
        {
            if (string.IsNullOrEmpty(this.Type))
                return false;
            if (string.IsNullOrEmpty(storage.Type))
                return false;
            return this.Type.Equals(storage.Type);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Storage && Equals((Storage)obj);
        }

        public override int GetHashCode()
        {
            return string.IsNullOrEmpty(this.Type) ? base.GetHashCode() : this.Type.GetHashCode();
        }
    }
}
