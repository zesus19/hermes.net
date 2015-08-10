using System;
using System.Collections.Concurrent;
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
using Arch.CMessaging.Client.Core.Collections;
using Arch.CMessaging.Client.Core.Future;
using Arch.CMessaging.Client.Core.Result;
using Arch.CMessaging.Client.Producer.Build;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Threading;
namespace Producer
{
    class Program
    {
        static void Main(string[] args)
        {
            BlockingQueue<int> bq = new BlockingQueue<int>(10000);
            var putCount = 0;
            var takeCount = 0;
            var timeoutCount = 0;
            new Task(() =>
            {
                while (true)
                {
                    Console.WriteLine("{0}_{1}_{2}", putCount, takeCount, timeoutCount);
                    Thread.Sleep(200);
                }
            }).Start();

            new ConcurrentRunner(10, 1).Run((i) =>
                {
                    if (i % 2 == 0)
                    {
                        while (true)
                        {
                            bq.Take();
                            Interlocked.Increment(ref takeCount);
                        }
                    }
                    else
                    {
                        for (int j = 0; j < 100000000; j++)
                        {
                            if (!bq.Put(i, 1)) Interlocked.Increment(ref timeoutCount);
                            else Interlocked.Increment(ref putCount);
                        }
                    }
                });

            Console.ReadLine();
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