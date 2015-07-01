using System;
using Freeway.Logging;
using Arch.CMessaging.Client.Core.Ioc;
using Arch.CMessaging.Client.Core.Env;
using Arch.CMessaging.Client.MetaEntity.Entity;
using Arch.CMessaging.Client.Core.MetaService.Remote;

namespace Arch.CMessaging.Client.Core.MetaService.Internal
{
	public class DefaultMetaManager : IMetaManager, IInitializable
	{
		private static readonly ILog log = LogManager.GetLogger (typeof(DefaultMetaManager));

		[Inject(LocalMetaLoader.ID)]
		private IMetaLoader m_localMeta;

		[Inject(RemoteMetaLoader.ID)]
		private IMetaLoader m_remoteMeta;

		[Inject]
		private IClientEnvironment m_env;

		[Inject(LocalMetaProxy.ID)]
		private IMetaProxy m_localMetaProxy;

		[Inject(RemoteMetaProxy.ID)]
		private IMetaProxy m_remoteMetaProxy;

		private bool m_localMode = false;

		public IMetaProxy getMetaProxy ()
		{
			if (isLocalMode ()) {
				return m_localMetaProxy;
			} else {
				return m_remoteMetaProxy;
			}
		}

		public Meta loadMeta ()
		{
			if (isLocalMode ()) {
				return m_localMeta.load ();
			} else {
				return m_remoteMeta.load ();
			}
		}

		private bool isLocalMode ()
		{
			return m_localMode;
		}

		public void Initialize ()
		{
			m_localMode = m_env.IsLocalMode ();

			if (m_localMode) {
				log.Info ("Meta manager started with local mode");
			} else {
				log.Info ("Meta manager started with remote mode");
			}
		}
	}
}

