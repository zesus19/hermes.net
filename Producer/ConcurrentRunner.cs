using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Producer
{
    public class ConcurrentRunner
    {
        private SemaphoreSlim semaphore;
        private CountdownEvent countdown;
        private int maxThread;
        private int loopEach;
        public ConcurrentRunner(int maxThread, int loopEach)
        {
            this.maxThread = maxThread;
            this.loopEach = loopEach;
            this.semaphore = new SemaphoreSlim(0, maxThread);
            this.countdown = new CountdownEvent(maxThread);
        }

        public void Run(Action<int> action)
        {
            int readyCount = 0;
            int overCount = 0;
            for (int i = 1; i <= maxThread; i++)
            {
                new Thread((_) =>
                {
                    Interlocked.Increment(ref readyCount);
                    Console.WriteLine("thread -> {0} gets ready. total -> {1}", Thread.CurrentThread.ManagedThreadId, readyCount);
                    semaphore.Wait();
                    for (int j = 0; j < loopEach; j++)
                    {
                        action(Convert.ToInt32(_));
                    }
                    Console.WriteLine("thread -> {0} runs an end. total left -> {1}", Thread.CurrentThread.ManagedThreadId, overCount);
                    Interlocked.Increment(ref overCount);
                    countdown.Signal();
                }).Start(i);
            }

            new Thread(() =>
            {
                while (true)
                {
                    if (Interlocked.CompareExchange(ref readyCount, 0, maxThread) == maxThread)
                    {
                        semaphore.Release(maxThread);
                        break;
                    }
                }
            }).Start();

            countdown.Wait();
            Console.WriteLine("run over, total threads -> {0}", maxThread);
        }
    }
}
