using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Arch.CMessaging.Client.Core.Future;
using Freeway.Logging;

namespace Arch.CMessaging.Client.Producer.Monitor
{
    public class DefaultSendMessageAcceptanceMonitor : ISendMessageAcceptanceMonitor
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DefaultSendMessageAcceptanceMonitor));
        private Dictionary<long, CancelableFuture> futures = new Dictionary<long, CancelableFuture>();
        private object syncRoot = new object();
        #region ISendMessageAcceptanceMonitor Members

        public IFuture<bool> Monitor(long correlationID)
        {
            var future = new CancelableFuture(correlationID, syncRoot, futures);
            lock (syncRoot)
            {
                futures[correlationID] = future;
            }
            return future;
        }

        public void Recieved(long correlationID, bool success)
        {
            Debug.WriteLine("Broker acceptance result is {0} for correlationId {1}", success, correlationID);
            lock (syncRoot)
            {
                CancelableFuture future = null;
                if (futures.TryGetValue(correlationID, out future))
                {
                    futures.Remove(correlationID);
                    if (future != null) future.Set(success);
                }
            }
        }

        #endregion

        private class CancelableFuture : AbstractFuture<bool>
        {
            private object syncRoot;
            private long correlationID;
            private Dictionary<long, CancelableFuture> futures;
            public CancelableFuture(long correlationID, object syncRoot, Dictionary<long, CancelableFuture> futures)
            {
                this.futures = futures;
                this.syncRoot = syncRoot;
                this.correlationID = correlationID;
            }

            public void Set(bool val)
            {
                base.Value = val;
            }

            public void SetException(Exception ex)
            {
                base.Value = ex;
            }

            public override bool Cancel(bool mayInterruptIfRunning)
            {
                base.Cancel(mayInterruptIfRunning);
                lock (syncRoot)
                {
                    futures.Remove(correlationID);
                }
                return true;
            }
        }
    }
}
