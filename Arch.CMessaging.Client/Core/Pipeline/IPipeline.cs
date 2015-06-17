using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arch.CMessaging.Client.Core.Pipeline
{
    public interface IPipeline<T>
    {
        T Put(object message);
    }
}
