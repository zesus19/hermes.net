using System;
using Arch.CMessaging.Client.Transport.Command;
using Arch.CMessaging.Client.Net.Core.Buffer;
using Arch.CMessaging.Client.Core.Utils;
using Arch.CMessaging.Client.Core.Future;

namespace Arch.CMessaging.Client.Transport.Command
{
    public class PullMessageCommand : AbstractCommand
    {
        private static long serialVersionUID = 8392887545356755515L;

        private SettableFuture<PullMessageResultCommand> m_future;

        public PullMessageCommand()
            : this(null, -1, null, 0, -1L)
        {
        }

        public PullMessageCommand(String topic, int partition, String groupId, int size, long expireTime)
            : base(CommandType.MessagePull)
        {
            Topic = topic;
            Partition = partition;
            GroupId = groupId;
            Size = size;
            ExpireTime = expireTime;
        }

        public string GroupId { get; private set; }

        public string Topic { get; private set; }

        public int Partition { get; private set; }

        public int Size { get; private set; }

        public long ExpireTime { get; private set; }

        public SettableFuture<PullMessageResultCommand> getFuture()
        {
            return m_future;
        }

        public void setFuture(SettableFuture<PullMessageResultCommand> future)
        {
            m_future = future;
        }


        public void OnResultReceived(PullMessageResultCommand ack)
        {
            m_future.Set(ack);
        }

        protected override void Parse0(IoBuffer buf)
        {
            throw new NotImplementedException();
        }

        protected override void  ToBytes0(IoBuffer buf)
        {
            var codec = new HermesPrimitiveCodec(buf);

            codec.WriteString(Topic);
            codec.WriteInt(Partition);
            codec.WriteString(GroupId);
            codec.WriteInt(Size);
            codec.WriteLong(ExpireTime);
        }


    }
}

