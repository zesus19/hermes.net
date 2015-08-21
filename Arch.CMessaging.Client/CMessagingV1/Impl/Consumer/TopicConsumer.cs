using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Arch.CMessaging.Client.API;
using Arch.CMessaging.Core.Content;
using Arch.CMessaging.Core.Log;
using Arch.CMessaging.Core.Util;
using System.IO;
using Arch.CMessaging.Client.Impl.Consumer.Check;

namespace Arch.CMessaging.Client.Impl.Consumer
{
    public class TopicConsumer : AbstractConsumer, ITopicConsumer
    {
        private static ThreadSafe.Integer _topicCount = new ThreadSafe.Integer(0);

        private string _topic;
        private string _exchangeName;
        private string _queueName;
        private string _pullingRequestUri;
        private const string Format = "topic:{0}//{1}/{2}{3}{4}";
        

        public TopicConsumer(IConsumerBuffer buffer) : base(buffer) { }

        public string HeaderFilter
        {
            get { return null; }
        }

        public void TopicBind(string topic, string exchangeName, string queueName = null)
        {
            Guard.ArgumentNotNullOrEmpty(Identifier, "Identifier");
            Guard.ArgumentNotNullOrEmpty(topic, "topic");
            Guard.ArgumentNotNullOrEmpty(exchangeName, "exchangeName");

            var isChange = false;
            if (_topic != topic)
            {
                checkTopic(topic);
                _topic = topic;
                isChange = true;
            }
            if (_exchangeName != exchangeName)
            {
                _exchangeName = exchangeName;
                isChange = true;
            }
            if (_queueName != queueName)
            {
                _queueName = queueName;
                isChange = true;
            }
            if(string.IsNullOrEmpty(_queueName))
            {
                _queueName = new HexStringConverter()
                    .ToString(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(topic + exchangeName + Identifier)));
            }

            if (!isChange) return;
            ConfigUtil.Instance.RegisterConsumer(ConfigKey);//添加
            if (!string.IsNullOrEmpty(PullingRequestUri)) ChannelConsumerCountor.RemoveConsumer(PullingRequestUri);//移除CONSUMER记数
            _pullingRequestUri = string.Format(Format, _topic, _exchangeName, Identifier,
                                               string.IsNullOrEmpty(_queueName) ? "" : "/" + _queueName,
                                               string.IsNullOrEmpty(HeaderFilter) ? "" : "?filter=" + HeaderFilter);
            Buffer.PullingRequestUri = _pullingRequestUri;
            Buffer.BatchSize = (int)this.BatchSize;
            Buffer.ReceiveTimeout = (int)this.ReceiveTimeout;
            ChannelConsumerCountor.AddConsumer(PullingRequestUri);//添加CONSUMER记数

#if DEBUG
                var directory = AppDomain.CurrentDomain.BaseDirectory;
                using (var sw = System.IO.File.AppendText(Path.Combine(directory, @"list.txt")))
                {
                    sw.WriteLine("{0}------>{1}", PullingRequestUri,
                                 new HexStringConverter().ToString(
                                     MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(PullingRequestUri))));
                }
#endif

        }

        protected override string PullingRequestUri
        {
            get { return _pullingRequestUri; }
        }
        protected override string ExchangeName
        {
            get { return _exchangeName; }
        }

        private void checkTopic(string topic)
        {
            TopicCheck.Check(topic);
        }
    }
}
