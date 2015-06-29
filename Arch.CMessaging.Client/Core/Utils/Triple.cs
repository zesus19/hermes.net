using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arch.CMessaging.Client.Core.Utils
{
    public class Triple<F, M, L>
    {
        public Triple() { }

        public Triple(F first, M middle, L last)
        {
            this.First = first;
            this.Middle = middle;
            this.Last = last;
        }

        public F First { get; set; }
        public M Middle { get; set; }
        public L Last { get; set; }

        public static Triple<F, M, L> From(F first, M middle, L last)
        {
            return new Triple<F, M, L>(first, middle, last);
        }

        public int Size { get { return 3; } }

        public T Get<T>(int index)
        {
            switch (index)
            {
                case 0:
                    return (T)(object)First;
                case 1:
                    return (T)(object)Middle;
                case 2:
                    return (T)(object)Last;
                default:
                    throw new ArgumentOutOfRangeException(string.Format("Index from 0 to {0}, but was {1}", Size, index));
            }
        }

        public bool Equals(Triple<F, M, L> triple)
        {
            if (First == null)
            {
                if (triple.First != null) return false;
            }
            else if (!First.Equals(triple.First)) return false;

            if (Middle == null)
            {
                if (triple.Middle != null) return false;
            }
            else if (!Middle.Equals(triple.Middle)) return false;

            if (Last == null)
            {
                if (triple.Last != null) return false;
            }
            else if (!Last.Equals(triple.Last)) return false;

            return true;
        } 

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Triple<F, M, L> && Equals((Triple<F, M, L>)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (this.First == null ? 0 : this.First.GetHashCode() * 97)
                    ^ (this.Middle == null ? 0 : this.Middle.GetHashCode() * 13)
                    ^ (this.Last == null ? 0 : this.Last.GetHashCode());
            }
        }
    }
}
