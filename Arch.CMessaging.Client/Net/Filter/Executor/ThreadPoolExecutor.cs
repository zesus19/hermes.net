using System;
using System.Threading.Tasks;

namespace Arch.CMessaging.Client.Net.Filter.Executor
{
    public class ThreadPoolExecutor : IExecutor
    {
        public void Execute(Action task)
        {
            Task.Factory.StartNew(task);
        }
    }
}
