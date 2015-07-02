using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arch.CMessaging.Client.Core.Result
{
    public interface ICompletionCallback<T>
    {
        void OnSuccess(T result);
        void OnFailure(Exception ex);
    }
}
