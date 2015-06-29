using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arch.CMessaging.Client.Core.Ioc;

namespace Arch.CMessaging.Client.Core.Env
{
    public class DefaultClientEnvironment : IClientEnvironment, IInitializable
    {
        #region IClientEnvironment Members

        public bool IsLocalMode()
        {
            throw new NotImplementedException();
        }

        public Env GetEnv()
        {
            throw new NotImplementedException();
        }

        public Utils.Properties GetProducerConfig(string topic)
        {
            throw new NotImplementedException();
        }

        public Utils.Properties GetConsumerConfig(string topic)
        {
            throw new NotImplementedException();
        }

        public Utils.Properties GetGlobalConfig()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IInitializable Members

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
