﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arch.CMessaging.Client.Core.Result
{
    public interface ICallback
    {
        void OnCompletion(SendResult sendResult, Exception exception);
    }
}
