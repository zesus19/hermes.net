using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arch.CMessaging.Client.Core.Ioc;
using Arch.CMessaging.Client.Core.Pipeline;
using Arch.CMessaging.Client.Producer.Build;

namespace Arch.CMessaging.Client.Producer.Pipeline
{
    //[Named(ServiceType=typeof(IValveRegistry), ServiceName=BuildConstants.PRODUCER)]
    public class ProducerValveRegistry : AbstractValveRegistry, IInitializable
    {

        #region IInitializable Members

        public void Initialize()
        {
            DoRegister(EnrichMessageValve.ID, 0);
            DoRegister(TracingMessageValve.ID, 1);
        }

        #endregion
    }
}
