using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arch.CMessaging.Client.Core.Ioc;
using Arch.CMessaging.Client.Core.Utils;

namespace Arch.CMessaging.Client.Transport.Command.Processor
{
	[Named (ServiceType = typeof(ICommandProcessorRegistry))]
	public class DefaultCommandProcessorRegistry : ICommandProcessorRegistry, IInitializable
	{
		private Dictionary<CommandType, ICommandProcessor> processors = new Dictionary<CommandType, ICommandProcessor> ();

		#region ICommandProcessorRegistry Members

		public void RegisterProcessor (CommandType type, ICommandProcessor processor)
		{
			if (processors.ContainsKey (type))
				throw new ArgumentException (string.Format ("Command processor for type {0} is already registered", type));

			if (processor != null)
				processors [type] = processor;
		}

		public ICommandProcessor FindProcessor (CommandType type)
		{
			ICommandProcessor processor = null;
			processors.TryGetValue (type, out processor);
			return processor;
		}

		public HashSet<ICommandProcessor> ListAllProcessors ()
		{
			return new HashSet<ICommandProcessor> (processors.Values);
		}

		#endregion

		#region IInitializable Members

		public void Initialize ()
		{
			var list = ComponentLocator.LookupList<ICommandProcessor> ();
			foreach (var processor in list) {
				foreach (var type in processor.CommandTypes()) {
					RegisterProcessor (type, processor);
				}
			}
		}

		#endregion
	}
}
