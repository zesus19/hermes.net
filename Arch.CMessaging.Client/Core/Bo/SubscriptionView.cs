using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arch.CMessaging.Client.Core.Bo
{
    public class SubscriptionView
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string Topic { get; set; }
        public string Group { get; set; }
        public string Endpoints { get; set; }
        public bool Equals(SubscriptionView view)
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(view.Name)) return false;
            if (string.IsNullOrEmpty(Topic) || string.IsNullOrEmpty(view.Topic)) return false;
            if (string.IsNullOrEmpty(Group) || string.IsNullOrEmpty(Group)) return false;
            if (string.IsNullOrEmpty(Endpoints) || string.IsNullOrEmpty(Endpoints)) return false;

            return this.ID == view.ID && this.Name.Equals(view.Name)
                && this.Topic.Equals(view.Topic) && this.Group.Equals(view.Group) 
                && this.Endpoints.Equals(view.Endpoints);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is SubscriptionView && Equals((SubscriptionView)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (this.ID.GetHashCode() * 397)
                    ^ ((string.IsNullOrEmpty(this.Name) ? 0 : this.Name.GetHashCode()) * 137)
                    ^ ((string.IsNullOrEmpty(this.Topic) ? 0 : this.Topic.GetHashCode() * 97))
                    ^ ((string.IsNullOrEmpty(this.Group) ? 0 : this.Group.GetHashCode() * 13))
                    ^ ((string.IsNullOrEmpty(this.Endpoints) ? 0 : this.Endpoints.GetHashCode()));
            }
        }
    }
}
