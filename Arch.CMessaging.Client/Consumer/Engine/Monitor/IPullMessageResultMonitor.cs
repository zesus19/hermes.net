using System;
using Arch.CMessaging.Client.Transport.Command;

namespace Arch.CMessaging.Client.Consumer.Engine.Monitor
{
	public interface IPullMessageResultMonitor
	{
		void monitor (PullMessageCommand cmd);

		void resultReceived (PullMessageResultCommand ack);
	}
}

