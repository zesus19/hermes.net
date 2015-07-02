using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arch.CMessaging.Client.Core.Config;
using Arch.CMessaging.Client.Core.Ioc;
using Arch.CMessaging.Client.Core.Collections;
using Freeway.Logging;

namespace Arch.CMessaging.Client.Transport.Command.Processor
{
	[Named (ServiceType = typeof(CommandProcessorManager))]
	public class CommandProcessorManager : IInitializable
	{
		private static readonly ILog log = LogManager.GetLogger (typeof(CommandProcessorManager));

		[Inject]
		private CoreConfig config;
		[Inject]
		private ICommandProcessorRegistry registry;

		private const int MAX_CAPACITY = 10000;
		private ProducerConsumer<CommandProcessorContext> producer;

		public void Initialize ()
		{
			this.producer = new ProducerConsumer<CommandProcessorContext> (MAX_CAPACITY, config.CommandProcessorThreadCount);
			this.producer.OnConsume += producer_OnConsume;
		}

		void producer_OnConsume (object sender, ConsumeEventArgs e)
		{
			var ctx = (e.ConsumingItem as SingleConsumingItem<CommandProcessorContext>).Item;
			var command = ctx.Command;
			var commandType = command.Header.CommandType.Value;
			var processor = registry.FindProcessor (commandType);
			if (processor == null)
				log.Error (string.Format ("Command processor not found for type {0}", commandType));
			else {
				try {
					processor.Process (ctx);
				} catch (Exception ex) {
					log.Error (ex);
				}
			}
		}

		public void Offer (CommandProcessorContext ctx)
		{
			producer.Produce (ctx);
		}
	}
}
