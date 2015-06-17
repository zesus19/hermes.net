using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arch.CMessaging.Client.Core.Message;
using Arch.CMessaging.Client.Net.Core.Buffer;
using Arch.CMessaging.Client.Net.Filter.Codec;
using Arch.CMessaging.Client.Net.Transport.Socket;
using Arch.CMessaging.Client.Transport;
using Arch.CMessaging.Client.Transport.Command;
using Arch.CMessaging.Client.Core.Utils;
using Arch.CMessaging.Client.Net;
using Arch.CMessaging.Client.Net.Core.Session;
using Arch.CMessaging.Client.Transport.EndPoint;

namespace Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var future = new Bootstrap()
                .Option(SessionOption.SO_KEEPALIVE, true)
                .Option(SessionOption.CONNECT_TIMEOUT_MILLIS, 5000)
                .Option(SessionOption.TCP_NODELAY, true)
                .Option(SessionOption.SO_SNDBUF, 4096)
                .Option(SessionOption.SO_RCVBUF, 4096)
                .Handler(chain =>
                {
                    chain.AddLast(
                        new ProtocolCodecFilter(new CommandCodecFactory()));
                })
                .Handler(new DefaultClientChannelInboundHandler(null, null, null, null, null))
                .Connect(args[0], 4376);
            future.Await();
            var session = future.Session;
            Console.ReadLine();
        }
    }
}
