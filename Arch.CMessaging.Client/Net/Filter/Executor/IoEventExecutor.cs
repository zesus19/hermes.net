using Arch.CMessaging.Client.Net.Core.Session;

namespace Arch.CMessaging.Client.Net.Filter.Executor
{
    public interface IoEventExecutor
    {
        void Execute(IoEvent ioe);
    }
}
