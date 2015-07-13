using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arch.CMessaging.Client.Newtonsoft.Json;

namespace Arch.CMessaging.Client.Core.Bo
{
    public class Tpg
    {
        public Tpg() { }
        public Tpg(string topic, int partition, string groupID)
        {
            this.Topic = topic;
            this.Partition = partition;
            this.GroupID = groupID;
        }

        [JsonProperty(PropertyName = "partition")]
        public int Partition { get; set; }
        [JsonProperty(PropertyName = "topic")]
        public string Topic { get; set; }
        [JsonProperty(PropertyName = "groupId")]
        public string GroupID { get; set; }

        public bool Equals(Tpg tpg)
        {
            if (string.IsNullOrEmpty(this.GroupID)
                || string.IsNullOrEmpty(tpg.GroupID))
            {
                return false;
            }

            if (string.IsNullOrEmpty(Topic)
                || string.IsNullOrEmpty(tpg.Topic))
            {
                return false;
            }

            return this.Partition == tpg.Partition
                && this.Topic.Equals(tpg.Topic) && this.GroupID.Equals(tpg.GroupID);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Tpg && Equals((Tpg)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (this.Partition * 193)
                    ^ ((string.IsNullOrEmpty(this.Topic) ? 0 : this.Topic.GetHashCode()) * 13)
                    ^ (string.IsNullOrEmpty(this.GroupID) ? 0 : this.GroupID.GetHashCode());
            }
        }

        public override string ToString()
        {
            return "Tpg [m_topic=" + Topic + ", m_partition=" + Partition + ", m_groupId=" + GroupID + "]";
        }
    }
}
