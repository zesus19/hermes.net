using System;

namespace Arch.CMessaging.Client.Net.Filter.Executor
{
    public interface IExecutor
    {
        void Execute(Action task);
    }
}
