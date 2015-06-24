using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Arch.CMessaging.Client.Core.Message;
using Arch.CMessaging.Client.Net.Core.Buffer;
using Arch.CMessaging.Client.Core.Utils;
using Arch.CMessaging.Client.Core.Future;
using Arch.CMessaging.Client.Core.Result;

namespace Arch.CMessaging.Client.Transport.Command
{
    public class SendMessageCommand : AbstractCommand
    {
        private int msgCounter;
        private const long serialVersionUID = 8443575812437722822L;
        private ThreadSafe.Long expireTime;
        private ConcurrentDictionary<int, IList<ProducerMessage>> messages;
        private Dictionary<int, IFuture<SendResult>> futures;

        public SendMessageCommand() : this(null, 0) { }

        public SendMessageCommand(string topic, int partition)
            : base(CommandType.MessageSend)
        {
            this.Topic = topic;
            this.Partition = partition;
            this.expireTime = new ThreadSafe.Long(0);
            this.messages = new ConcurrentDictionary<int, IList<ProducerMessage>>();
            this.futures = new Dictionary<int, IFuture<SendResult>>();
        }

        public string Topic { get; private set; }
        public int Partition { get; private set; }
        public long ExpireTime 
        {
            get { return expireTime.ReadFullFence(); }
            set { expireTime.WriteFullFence(value); }
        }

        public void AddMessage(ProducerMessage message, IFuture<SendResult> future)
        {
            Validate(message);
            message.SequenceNo = Interlocked.Increment(ref msgCounter);
            if (message.IsPriority)
            {
                messages.TryAdd(0, new List<ProducerMessage>());
                messages[0].Add(message);
            }
            else
            {
                messages.TryAdd(1, new List<ProducerMessage>());
                messages[1].Add(message);
            }

            futures[message.SequenceNo] = future;
        }

        protected override void Parse0(IoBuffer buf)
        {
            throw new NotImplementedException();
        }

        protected override void ToBytes0(IoBuffer buf)
        {
            var codec = new HermesPrimitiveCodec(buf);
            codec.WriteInt(Thread.VolatileRead(ref msgCounter));
            codec.WriteString(Topic);
            codec.WriteInt(Partition);
            WriteDatas(buf, codec, messages);
        }

        private void WriteDatas(IoBuffer buf, HermesPrimitiveCodec codec, IDictionary<int, IList<ProducerMessage>> messages)
        {
            codec.WriteInt(messages.Count);
            foreach (var kvp in messages)
            {
                codec.WriteInt(kvp.Key);
                WriteMessages(kvp.Value, codec, buf);
            }
        }

        private void WriteMessages(IList<ProducerMessage> list, HermesPrimitiveCodec codec, IoBuffer buf)
        {
            var msgCodec = ComponentLocator.Lookup<IMessageCodec>();
            codec.WriteInt(list.Count);

            //seqNos
            foreach (var message in list) codec.WriteInt(message.SequenceNo);

            // placeholder for payload len
            var indexBeforeLen = buf.Position;
            codec.WriteInt(-1);

            var indexBeforePayload = buf.Position;
            //payload
            foreach (var message in list) msgCodec.Encode(message, buf);
            var indexAfterPayload = buf.Position;
            var payloadLen = indexAfterPayload - indexBeforePayload;

            // refill payload len
            buf.Position = indexBeforeLen;
            codec.WriteInt(payloadLen);

            buf.Position = indexAfterPayload;
        }

        private void Validate(ProducerMessage message)
        {
            if (string.IsNullOrEmpty(Topic))
                throw new ArgumentNullException("SendMessageCommand.Topic");

            if (string.IsNullOrEmpty(message.Topic))
                throw new ArgumentNullException("ProducerMessage.Topic");

            if (!Topic.Equals(message.Topic, StringComparison.OrdinalIgnoreCase)
                || Partition != message.Partition)
            {
                throw new ArgumentException(string.Format("Illegal message[topic={0}, partition={1}] try to add to SendMessageCommand[topic={0}, partition={1}]",
                    message.Topic, message.Partition, Topic, Partition));
            }
        }
    }
}
