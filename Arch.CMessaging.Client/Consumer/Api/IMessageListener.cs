using System;
using System.Collections.Generic;
using Arch.CMessaging.Client.Core.Message;

namespace Arch.CMessaging.Client.Consumer.Api
{
	public interface IMessageListener<T>
	{
		void onMessage (List<IConsumerMessage> messages);
	}
}

