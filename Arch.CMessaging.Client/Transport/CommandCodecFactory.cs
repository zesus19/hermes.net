using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arch.CMessaging.Client.Transport.Command;
using Arch.CMessaging.Client.Net.Filter.Codec.Demux;

namespace Arch.CMessaging.Client.Transport
{
    public class CommandCodecFactory : DemuxingProtocolCodecFactory
    {
        public CommandCodecFactory()
        {
            AddMessageDecoder<CommandDecoder>();
            AddMessageEncoder<ICommand, CommandEncoder>();
        }
    }
}
