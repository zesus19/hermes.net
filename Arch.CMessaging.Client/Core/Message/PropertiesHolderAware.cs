using System;

namespace Arch.CMessaging.Client.Core.Message
{
    public interface PropertiesHolderAware
    {
        PropertiesHolder PropertiesHolder { get; private set; }
    }
}

