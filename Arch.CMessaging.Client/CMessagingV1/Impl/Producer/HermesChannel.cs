using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arch.CMessaging.Client.API;

namespace Arch.CMessaging.Client.Impl.Producer
{
    public class HermesChannel : IProducerChannel
    {
        #region IProducerChannel Members

        public IMessageProducer CreateProducer()
        {
            return new HermesProducer();
        }

        #endregion

        #region IMessageChannel Members

        public event Event.ChannelOutOfCapacityEventHandler OutOfCapacity;

        public System.Collections.IDictionary ClientProperties { get; private set; }

        public System.Collections.IDictionary ServerProperties { get; private set; }

        public bool EnableMonitoring { get; set; }

        public ushort ConnectionMax { get; set; }

        public int AckTimeout { get; set; }

        public uint Capacity { get; set; }

        public uint CurrentSize { get; private set; }

        public bool IsReliable { get; private set; }
        public bool IsInOrder { get; private set; }

        public string Uri { get; private set; }

        public string[] KnownHosts { get; private set; }

        public bool IsOpen { get; private set; }

        public void Open(int timeout)
        {
            
        }

        public void Close(int timeout)
        {
            
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            
        }

        #endregion
    }
}
