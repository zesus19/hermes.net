using System;

namespace Arch.CMessaging.Client.Net.Core.Filterchain
{

    public interface IEntry<TFilter, TNextFilter>
    {
        String Name { get; }
        TFilter Filter { get; }
        TNextFilter NextFilter { get; }
        void AddBefore(String name, TFilter filter);
        void AddAfter(String name, TFilter filter);
        void Replace(TFilter newFilter);
        void Remove();
    }
}
