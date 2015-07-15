using System;
using Arch.CMessaging.Client.Net.Core.Buffer;
using System.Collections.Generic;
using Arch.CMessaging.Client.Transport;

namespace Arch.CMessaging.Client
{
    public class TppConsumerMessageBatch
    {
        public String Topic { get; set; }

        public int Partition { get; set; }

        public bool Resend { get; set; }

        public int Priority { get; set; }

        public List<MessageMeta> MessageMetas { get; private set; }

        public ITransferCallback TransferCallback { get; private set; }

        public IoBuffer Data { get; set; }

        public TppConsumerMessageBatch()
        {
            MessageMetas = new List<MessageMeta>();
        }

        public bool IsPriority()
        {
            return Priority == 0;
        }

        public void AddMessageMeta(MessageMeta msgMeta)
        {
            MessageMetas.Add(msgMeta);
        }

        public void AddMessageMetas(List<MessageMeta> msgMetas)
        {
            MessageMetas.AddRange(msgMetas);
        }
    }

    public class MessageMeta
    {
        public long Id;

        public int RemainingRetries;

        public long OriginId;

        public int Priority;

        public bool Resend;

        public MessageMeta(long id, int remainingRetries, long originId, int priority, bool isResend)
        {
            Id = id;
            RemainingRetries = remainingRetries;
            OriginId = originId;
            Priority = priority;
            Resend = isResend;
        }

    }
}

