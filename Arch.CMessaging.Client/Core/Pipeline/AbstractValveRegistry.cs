using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Arch.CMessaging.Client.Core.Pipeline.spi;
using Arch.CMessaging.Client.Core.Utils;

namespace Arch.CMessaging.Client.Core.Pipeline
{
    public abstract class AbstractValveRegistry : IValveRegistry
    {
        private IList<IValve> valves;
        private SortedSet<Triple<IValve, string, int>> tuples;

        protected AbstractValveRegistry()
        {
            this.valves = new List<IValve>();
            this.tuples = new SortedSet<Triple<IValve, string, int>>(new ValveComparer());
        }

        #region IValveRegistry Members

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Register(IValve valve, string name, int order)
        {
            tuples.Add(new Triple<IValve, string, int>(valve, name, order));
            valves = new List<IValve>(tuples.Select(t => t.First));
        }

        public IList<IValve> GetValveList()
        {
            return valves;
        }

        #endregion

        protected void DoRegister(string id, int order)
        {
            Register(ComponentLocator.Lookup<IValve>(id), id, order);
        }

        public class ValveComparer : IComparer<Triple<IValve, string, int>>
        {
            #region IComparer<Triple<IValve,string,int>> Members
            public int Compare(Triple<IValve, string, int> x, Triple<IValve, string, int> y)
            {
                return x.Last.CompareTo(y.Last);
            }

            #endregion
        }
    }
}
