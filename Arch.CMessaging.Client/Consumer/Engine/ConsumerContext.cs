using System;
using Arch.CMessaging.Client.MetaEntity.Entity;
using Arch.CMessaging.Client.Consumer.Api;

namespace Arch.CMessaging.Client.Consumer.Engine
{
	public class ConsumerContext
	{
		public Topic Topic { get; private set; }

		public String GroupId { get; private set; }

		public Type MessageClazz { get; private set; }

		public IMessageListener<Object> Consumer { get; private set; }

		public ConsumerType ConsumerType { get; private set; }

		public String SessionId { get; private set; }

		public ConsumerContext ()
		{
			SessionId = Guid.NewGuid ().ToString ();
		}
	}
}

