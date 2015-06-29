using System;
using Arch.CMessaging.Client.Core.Message;
using System.Collections.Generic;

namespace Arch.CMessaging.Client.Consumer.Engine.Notifier
{
	public interface IConsumerNotifier
	{
		void Register (long correlationId, ConsumerContext consumerContext);

		void Deregister (long correlationId);

		void MessageReceived (long correlationId, List<IConsumerMessage<Object>> msgs);

		ConsumerContext Find (long correlationId);
	}
}

