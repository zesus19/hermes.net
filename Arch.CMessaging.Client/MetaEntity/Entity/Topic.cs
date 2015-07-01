using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arch.CMessaging.Client.Newtonsoft.Json;
using Arch.CMessaging.Client.MetaEntity.Transform;

namespace Arch.CMessaging.Client.MetaEntity.Entity
{
    [Serializable]
    public class Topic
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string StorageType { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
		[JsonConverter(typeof(MillSecondEpochConverter))]
        public DateTime CreateTime { get; set; }
		[JsonConverter(typeof(MillSecondEpochConverter))]
        public DateTime LastModifiedTime { get; set; }
        public long SchemaID { get; set; }
        public string ConsumerRetryPolicy { get; set; }
        public string CreateBy { get; set; }
        public string EndpointType { get; set; }
        public int AckTimeoutSeconds { get; set; }
        public string CodecType { get; set; }
        public string OtherInfo { get; set; }
        public List<ConsumerGroup> ConsumerGroups { get; set; }
        public List<Producer> Producers { get; set; }
        public List<Partition> Partitions { get; set; }
        public List<Property> Properties { get; set; }

        public Topic()
            : this(null)
        { }

        public Topic(string name)
        {
            this.Name = name;
            this.ConsumerGroups = new List<ConsumerGroup>();
            this.Producers = new List<Producer>();
            this.Partitions = new List<Partition>();
            this.Properties = new List<Property>();
        }

        public Topic AddConsumerGroup(ConsumerGroup consumerGroup)
        {
            ConsumerGroups.Add(consumerGroup);
            return this;
        }

        public ConsumerGroup FindConsumerGroup(string name)
        {
            ConsumerGroup consumerGroup = null;
            if (!string.IsNullOrEmpty(name))
            {
                consumerGroup = ConsumerGroups.Find(g => name.Equals(g.Name));
            }
            return consumerGroup;
        }

        public bool RemoveConsumerGroup(string name)
        {
            if (string.IsNullOrEmpty(name)) return false;
            return ConsumerGroups.RemoveAll(c => name.Equals(c.Name)) > 0;
        }

        public Topic AddPartition(Partition partition)
        {
            Partitions.Add(partition);
            return this;
        }

        public Partition FindPartition(int id)
        {
            return Partitions.Find(p => id == p.ID);
        }

        public bool RemovePartition(int id)
        {
            return Partitions.RemoveAll(p => id == p.ID) > 0;
        }

        public Topic AddProducer(Producer producer)
        {
            Producers.Add(producer);
            return this;
        }

        public Topic AddProperty(Property property)
        {
            Properties.Add(property);
            return this;
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

        public bool Equals(Topic topic)
        {
            if (string.IsNullOrEmpty(this.Name))
                return false;
            return this.Name.Equals(topic.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Topic && Equals((Topic)obj);
        }

        public override int GetHashCode()
        {
            return string.IsNullOrEmpty(this.Name) ? base.GetHashCode() : this.Name.GetHashCode();
        }
    }     
}
