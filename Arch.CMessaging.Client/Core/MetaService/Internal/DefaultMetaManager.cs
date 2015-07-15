using System;
using Freeway.Logging;
using Arch.CMessaging.Client.Core.Ioc;
using Arch.CMessaging.Client.Core.Env;
using Arch.CMessaging.Client.MetaEntity.Entity;
using Arch.CMessaging.Client.Core.MetaService.Remote;

namespace Arch.CMessaging.Client.Core.MetaService.Internal
{
    [Named(ServiceType = typeof(IMetaManager))]
    public class DefaultMetaManager : IMetaManager, IInitializable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DefaultMetaManager));

        [Inject(LocalMetaLoader.ID)]
        private IMetaLoader localMeta;

        [Inject(RemoteMetaLoader.ID)]
        private IMetaLoader remoteMeta;

        [Inject]
        private IClientEnvironment m_env;

        [Inject(LocalMetaProxy.ID)]
        private IMetaProxy localMetaProxy;

        [Inject(RemoteMetaProxy.ID)]
        private IMetaProxy remoteMetaProxy;

        private bool localMode = false;

        public IMetaProxy GetMetaProxy()
        {
            if (IsLocalMode())
            {
                return localMetaProxy;
            }
            else
            {
                return remoteMetaProxy;
            }
        }

        public Meta LoadMeta()
        {
            if (IsLocalMode())
            {
                return localMeta.Load();
            }
            else
            {
                return remoteMeta.Load();
            }
        }

        private bool IsLocalMode()
        {
            return localMode;
        }

        public void Initialize()
        {
            localMode = m_env.IsLocalMode();

            if (localMode)
            {
                log.Info("Meta manager started with local mode");
            }
            else
            {
                log.Info("Meta manager started with remote mode");
            }
        }
    }
}

