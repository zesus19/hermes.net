using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arch.CMessaging.Client.Net.Core.Filterchain
{
    public class RangeIoFilterChainBuilder
    {
        private DefaultIoFilterChainBuilder builder;
        public RangeIoFilterChainBuilder(DefaultIoFilterChainBuilder builder)
        {
            this.builder = builder;
        }

        public void AddLast(params IoFilter[] filters)
        {
            if (filters != null)
            {
                for (int i = 0; i < filters.Length; i++)
                {
                    var filter = filters[i];
                    if (filter != null)
                        builder.AddLast(string.Format("{0}-{1}", filter.GetType().Name, i), filter);
                }
            }
        }
    }
}
