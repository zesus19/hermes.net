using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arch.CMessaging.Client.Core.Future;
using Arch.CMessaging.Client.Core.Pipeline;
using Arch.CMessaging.Client.Core.Result;

namespace Arch.CMessaging.Client.Producer.Pipeline
{
    public interface IProducerPipelineSinkManager
    {
        IPipelineSink GetSink(string topic);
    }
}
