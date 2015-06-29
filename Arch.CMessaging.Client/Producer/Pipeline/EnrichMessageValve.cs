using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arch.CMessaging.Client.Core.Pipeline.spi;
using Arch.CMessaging.Client.Core.Pipeline;
using Arch.CMessaging.Client.Core.Ioc;

namespace Arch.CMessaging.Client.Producer.Pipeline
{
    [Named]
    public class EnrichMessageValve : IValve
    {
        public const string ID = "enrich";

        #region IValve Members

        public void Handle(IPipelineContext context, object payload)
        {
            //todo
        }

        #endregion
    }
}
