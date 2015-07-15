using System;
using System.Collections.Generic;

namespace Arch.CMessaging.Client.Consumer.Engine
{
    public class CompositeSubscribeHandle : ISubscribeHandle
    {
        private List<ISubscribeHandle> ChildHandles = new List<ISubscribeHandle>();

        public void AddSubscribeHandle(ISubscribeHandle handle)
        {
            ChildHandles.Add(handle);
        }

        public void Close()
        {
            foreach (ISubscribeHandle child in ChildHandles)
            {
                child.Close();
            }
        }
    }
}

