using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arch.CMessaging.Client.Core.Result
{
    public class SendResult
    {
        private bool success;
        public SendResult(bool success)
        {
            this.success = success;
        }

        public bool IsSuccess { get { return success; } }
    }
}
