using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arch.CMessaging.Client.Core.Pipeline.spi
{
    public interface IValve
    {
        void Handle(IPipelineContext context, object payload);
    }
}
