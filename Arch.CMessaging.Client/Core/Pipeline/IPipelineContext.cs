using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arch.CMessaging.Client.Core.Pipeline
{
    public interface IPipelineContext
    {
        object GetSource();
        void Next(object payload);
        void Put(string name, object value);
        object Get(string name);
        object GetResult();
    }
}
