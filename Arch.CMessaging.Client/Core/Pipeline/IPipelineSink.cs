﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arch.CMessaging.Client.Core.Pipeline
{
    public interface IPipelineSink<T>
    {
        T Handle(IPipelineContext context, object payload);
    }
}
