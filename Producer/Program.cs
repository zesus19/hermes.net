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
                int counter = 0;
                ComponentsConfigurator.DefineComponents();
                var p = Arch.CMessaging.Client.Producer.Producer.GetInstance();
                AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;

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
                        for (int i = 0; i < 10000;i++ )
                        {
                            try
                            {
                                var refKey = i.ToString();
                                var future = p.Message("cmessage_fws", "", string.Format("hello c#_{0}", i)).WithRefKey(refKey).Send();
                                //var future = p.Message("order_new", "", string.Format("hello c#_{0}", i)).WithRefKey(refKey).Send();
                                //var result = future.Get(8000);
                                Interlocked.Increment(ref counter);
                                //Thread.Sleep(1000);
                                //Console.WriteLine("aaa");
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

        static void CurrentDomain_FirstChanceException(object sender, FirstChanceExceptionEventArgs e)
        {
            //lock (typeof(Program))
            //{
            //    using (var writer = File.AppendText(@"c:\1.txt"))
            //    {
            //        writer.WriteLine(e.Exception.ToString());
            //    }
            //}

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