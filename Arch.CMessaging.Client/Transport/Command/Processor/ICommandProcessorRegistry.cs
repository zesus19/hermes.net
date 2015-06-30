using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Arch.CMessaging.Client.Transport.Command.Processor
{
    public interface ICommandProcessorRegistry
    {
        void RegisterProcessor(CommandType type, ICommandProcessor processor);
        ICommandProcessor FindProcessor(CommandType type);
        HashSet<ICommandProcessor> ListAllProcessors();
    }
}
