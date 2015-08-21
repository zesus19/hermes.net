using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Arch.CMessaging.Client.Core.Message;
using Arch.CMessaging.Client.Core.Message.Payload;
using Arch.CMessaging.Client.Newtonsoft.Json;
using Arch.CMessaging.Core.Content;
using Arch.CMessaging.Core.gen;

namespace Arch.CMessaging.Client.Impl.Consumer
{
    public class HermesMessageReader : IMessageReader
    {
        private Stream bodyStream;
        private IConsumerMessage message;
        private BasicHeader messageHeader;
        public HermesMessageReader(IConsumerMessage message)
        {
            this.message = message;
        }
        #region IMessageReader Members

        internal IConsumerMessage RawMessage { get { return message; } }

        public byte[] GetBinary()
        {
            if (HasMessage()) return message.GetBody<RawMessage>().EncodedMessage;
            else return new byte[0];
        }

        public TObject GetObject<TObject>()
        {
            var obj = default(TObject);
            if (HasMessage())
            {
                var body = message.GetBody<RawMessage>();
                var codec = PayloadCodecFactory.GetCodecByTopicName(message.Topic);
                obj = (TObject)codec.Decode(GetBinary(), typeof(TObject));
            }
            return obj;
        }

        public System.IO.Stream GetStream()
        {
            if (HasMessage())
                bodyStream = new MemoryStream(GetBinary());
            return bodyStream;
        }

        public string GetText()
        {
            return GetObject<string>();
        }

        public bool HasMessage()
        {
            return message != null;
        }

        public IHeaderProperties HeaderProperties
        {
            get
            {
                if (messageHeader == null)
                {
                    if (HasMessage())
                    {
                        messageHeader = new BasicHeader
                        {
                            AppID = message.GetProperty("appid"),
                            Compression = CompressionType.None,
                            ContentLength = GetBinary().Length,
                            MessageID = message.GetProperty("messageid"),
                            Serialization = SerializationType.Json,
                            Version = "2.0",
                            ExchangeName = message.GetProperty("exchangename"),
                            RawType = message.GetProperty("rawtype"),
                            Subject = message.GetProperty("subject"),
                            Type = MessageType.Object
                        };

                        var timestamp = 0L;
                        long.TryParse(message.GetProperty("timestamp"), out timestamp);
                        messageHeader.Timestamp = timestamp;
                        var userHeader = message.GetProperty("cmessage_userhead_#");
                        if (!string.IsNullOrEmpty(userHeader))
                            messageHeader.UserHeader = JsonConvert.DeserializeObject<Dictionary<string, string>>(userHeader);
                    }
                }

                return messageHeader;
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (bodyStream != null)
                bodyStream.Dispose();
        }

        #endregion
    }
}
