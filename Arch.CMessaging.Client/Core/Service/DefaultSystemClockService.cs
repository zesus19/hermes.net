using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arch.CMessaging.Client.Core.Config;
using Arch.CMessaging.Client.Core.Ioc;
using Arch.CMessaging.Client.Core.Utils;

namespace Arch.CMessaging.Client.Core.Service
{
	[Named (ServiceType = typeof(ISystemClockService))]
	public class DefaultSystemClockService : ISystemClockService, IInitializable
	{
		[Inject]
		private CoreConfig config;

		//[Inject]
		private RunningStatusStatisticsService runningStatusStatService;

		#region ISystemClockService Members

		public long Now ()
		{
			return DateTime.Now.CurrentTimeMillis ();
		}

		#endregion

		#region IInitializable Members

		public void Initialize ()
		{
		}

		#endregion
	}
}
