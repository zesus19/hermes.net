using System;
using System.Collections.Generic;
using Arch.CMessaging.Client.MetaEntity.Entity;
using Arch.CMessaging.Client.Consumer.Engine.Bootstrap.Strategy;

namespace Arch.CMessaging.Client.Consumer.Engine.Bootstrap
{
	public class BrokerConsumerBootstrap : BaseConsumerBootstrap
	{
		private IBrokerConsumptionStrategyRegistry ConsumptionStrategyRegistry;

		protected override ISubscribeHandle doStart (ConsumerContext context)
		{
			CompositeSubscribeHandle handler = new CompositeSubscribeHandle ();

			List<Partition> partitions = MetaService.ListPartitionsByTopic (context.Topic.Name);
			foreach (Partition partition in partitions) {
				handler.AddSubscribeHandle (ConsumptionStrategyRegistry.FindStrategy (context.ConsumerType).Start (
					context, partition.ID));
			}

			return handler;
		}
	}
}

