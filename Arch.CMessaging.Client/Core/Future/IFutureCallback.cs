using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arch.CMessaging.Client.Core.Future
{
    public interface IFutureCallback<T>
    {
        void OnSuccess(T success);
        void OnFailure(Exception ex);
    }
}
