using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arch.CMessaging.Client.Core.Pipeline.spi;

namespace Arch.CMessaging.Client.Core.Pipeline
{
    public interface IValveRegistry
    {
        void Register(IValve valve, string name, int order);

        IList<IValve> GetValveList();
    }
}
