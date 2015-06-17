using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arch.CMessaging.Client.Meta.Entity
{
    [Serializable]
    public class ConsumerGroup
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string AppIDs { get; set; }
        public string RetryPolicy { get; set; }
        public int AckTimeoutSeconds { get; set; }
        public bool OrderedConsume { get; set; }

        public ConsumerGroup() : this(null) { }

        public ConsumerGroup(string name)
        {
            this.Name = name;
            this.OrderedConsume = true;
        }

        public bool Equals(ConsumerGroup group)
        {
            if (string.IsNullOrEmpty(this.Name))
                return false;
            if (string.IsNullOrEmpty(group.Name))
                return false;
            return this.Name.Equals(group.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ConsumerGroup && Equals((ConsumerGroup)obj);
        }

        public override int GetHashCode()
        {
            return string.IsNullOrEmpty(this.Name) ? base.GetHashCode() : this.Name.GetHashCode();
        }
    }
}
