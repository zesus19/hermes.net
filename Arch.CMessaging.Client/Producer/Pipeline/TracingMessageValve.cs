using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arch.CMessaging.Client.Core.Pipeline.spi;
using Arch.CMessaging.Client.Core.Pipeline;
using Arch.CMessaging.Client.Core.Ioc;
using Arch.CMessaging.Client.Core.Message;
using Com.Dianping.Cat;
using Arch.CMessaging.Client.Core.Utils;

namespace Arch.CMessaging.Client.Producer.Pipeline
{
	[Named (ServiceType = typeof(IValve), ServiceName = TracingMessageValve.ID)]
	public class TracingMessageValve : IValve
	{
		public const string ID = "tracing";

		#region IValve Members

		public void Handle (IPipelineContext context, object payload)
		{
            var msg = payload as ProducerMessage;
            var topic = msg.Topic;
            var t = Cat.NewTransaction("Message.Produce.Tried", topic);
            t.AddData("key", msg.Key);

            var tree = Cat.GetThreadLocalMessageTree();
            try
            {
                

                var childMsgId = Cat.CreateMessageId();
                var rootMsgId = tree.RootMessageId;
                String msgId = Cat.GetThreadLocalMessageTree().MessageId;
                rootMsgId = rootMsgId == null ? msgId : rootMsgId;
                Cat.LogEvent("Message:" + topic, "Produced:" + Local.IPV4, CatConstants.SUCCESS, "key=" + msg.Key);
                Cat.LogEvent("Producer:" + Local.IPV4, topic, CatConstants.SUCCESS, "key=" + msg.Key);

                msg.AddDurableSysProperty(CatConstants.CURRENT_MESSAGE_ID, msgId);
                msg.AddDurableSysProperty(CatConstants.SERVER_MESSAGE_ID, childMsgId);
                msg.AddDurableSysProperty(CatConstants.ROOT_MESSAGE_ID, rootMsgId);
                Cat.LogEvent(CatConstants.TYPE_REMOTE_CALL, "", CatConstants.SUCCESS, childMsgId);

                context.Next(payload);

                t.Status = CatConstants.SUCCESS; 
            }
            catch (Exception ex)
            {
                Cat.LogError(ex);
                t.SetStatus(ex);
            }
            finally
            {
                t.Complete();
            }
		}

		#endregion
	}
}
