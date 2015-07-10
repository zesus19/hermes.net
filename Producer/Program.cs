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
            try
            {
                var buffer = IoBuffer.Allocate(100);
                buffer.Put(1);
                buffer.Put(2);
                buffer.Put(3);
                buffer.Put(4);
                buffer.Put(5);
                buffer.Put(6);
                buffer.Put(7);
                buffer.Put(8);
                buffer.Put(9);
                buffer.Put(10);

                buffer.Flip();
                var b = buffer.Get();
                b = buffer.Get();
                b = buffer.Get();

                var cc = buffer.GetRemainingArray();

                var d = buffer.GetSlice(3);
                var c = d.Get();
                c = d.Get();
                c = d.Get();


                


                long counter = 0;
                ComponentsConfigurator.DefineComponents();
                var p = Arch.CMessaging.Client.Producer.Producer.GetInstance();

                new Task(() =>
                {
                    while (true)
                    {
                        Console.WriteLine(counter);
                        Thread.Sleep(500);
                    }
                }).Start();

                new ConcurrentRunner(1, 1).Run((_) =>
                    {
                        for (var i = 0L; i < long.MaxValue;i++ )
                        {
                            try
                            {
                                var refKey = i.ToString();
                                //var future = p.Message("cmessage_fws", "", string.Format("hello c#_{0}", i)).WithRefKey(refKey).Send();
                                var future = p.Message("order_new", "", string.Format("hello c#_{0}", i)).WithRefKey(refKey).Send();
                                future.Get();
                                Console.WriteLine("aaa");
                                Interlocked.Increment(ref counter);
                                if (i % 100000 == 0) Thread.Sleep(5000);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }
                    });
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