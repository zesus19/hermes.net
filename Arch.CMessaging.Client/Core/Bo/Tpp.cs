using System;

namespace Arch.CMessaging.Client.Core.Bo
{
    public class Tpp
    {
        public String Topic{ get; set; }

        public int Partition{ get; set; }

        public bool Priority{ get; set; }

        public Tpp(String topic, int partition, bool priority)
        {
            this.Topic = topic;
            this.Partition = partition;
            this.Priority = priority;
        }

        public int getPriorityInt()
        {
            // TODO move to other place
            return Priority ? 0 : 1;
        }

        public override int GetHashCode()
        {
            int prime = 31;
            int result = 1;
            result = prime * result + Partition;
            result = prime * result + (Priority ? 1231 : 1237);
            result = prime * result + ((Topic == null) ? 0 : Topic.GetHashCode());
            return result;
        }

        public override bool Equals(Object obj)
        {
            if (this == obj)
                return true;
            if (obj == null)
                return false;
            if (GetType() != obj.GetType())
                return false;
            Tpp other = (Tpp)obj;
            if (Partition != other.Partition)
                return false;
            if (Priority != other.Priority)
                return false;
            if (Topic == null)
            {
                if (other.Topic != null)
                    return false;
            }
            else if (!Topic.Equals(other.Topic))
                return false;
            return true;
        }

        public override string ToString()
        {
            return "Tpp [m_topic=" + Topic + ", m_partition=" + Partition + ", m_priority=" + Priority + "]";
        }
    }
}

