using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

namespace Producer
{
    class Program
    {
        static void Main(string[] args)
        {
            try
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
                            new MagicNumberPrepender(),
                            new LengthFieldPrepender(4),
                            new ProtocolCodecFilter(new CommandCodecFactory()));
                    })
                    .Handler(new DefaultClientChannelInboundHandler(null, null, null, null, null))
                    .Connect(args[0], 4376);
                

                future.Await();
                var session = future.Session;

                var message = new ProducerMessage("cmessage_fws", 1233213423L) { Partition = 0, Key = "key", IsPriority = true, BornTime = DateTime.Now.CurrentTimeMillis() };
                message.PropertiesHolder.DurableProperties.Add("SYS.RootMessageId", "hermes-c0a8cc01-398166-30001");
                message.PropertiesHolder.DurableProperties.Add("SYS.ServerMessageId", "hermes-c0a8cc01-398166-30000");
                message.PropertiesHolder.DurableProperties.Add("SYS.CurrentMessageId", "hermes-c0a8cc01-398166-30001");
                var command = new SendMessageCommand("cmessage_fws", 0);
                command.AddMessage(message);

                var writeFuture = session.Write(command);
                writeFuture.Await();
                System.Threading.Thread.Sleep(1000);
                writeFuture = session.Write(command);
                writeFuture.Await();
                Console.ReadLine();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }
    }
}


//StringBuilder sb = new StringBuilder();
//buf.Flip();
//var seg = buf.GetRemaining();
//byte[] array = new byte[seg.Count];
//Array.Copy(seg.Array, seg.Offset, array, 0, seg.Count);

//foreach (var b in array)
//{
//    sb.Append(b.ToString() + "|");
//}
//string s = string.Empty;