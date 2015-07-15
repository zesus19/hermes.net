using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arch.CMessaging.Client.Net.Core.Buffer;
using Arch.CMessaging.Client.Core.Message.Payload;
using Arch.CMessaging.Client.Transport;
using Arch.CMessaging.Client.Core.Utils;

namespace Arch.CMessaging.Client.Core.Message.Codec.Internal
{
    public class MessageCodecBinaryV1Handler : IMessageCodecHandler
    {
        #region IMessageCodecHandler Members

        public void Encode(ProducerMessage message, IoBuffer buf)
        {
            var bodyCodec = PayloadCodecFactory.GetCodecByTopicName(message.Topic);
            var body = bodyCodec.Encode(message.Topic, message.GetBody<object>());
            Encode(message, buf, body, bodyCodec.Type);
        }

        public PartialDecodedMessage DecodePartial(IoBuffer buf)
        {
            HermesPrimitiveCodec codec = new HermesPrimitiveCodec(buf);

            // skip whole length
            codec.ReadInt();
            // skip header length
            int headerLen = codec.ReadInt();
            // skip body length
            int bodyLen = codec.ReadInt();
            verifyChecksum(buf, headerLen + bodyLen);
            PartialDecodedMessage msg = new PartialDecodedMessage();
            msg.Key = codec.ReadString();
            msg.BornTime = codec.ReadLong();
            msg.RemainingRetries = codec.ReadInt();
            msg.BodyCodecType = codec.ReadString();

            int len = codec.ReadInt();
            msg.DurableProperties = buf.GetSlice(len);

            len = codec.ReadInt();
            msg.VolatileProperties = buf.GetSlice(len);

            msg.Body = buf.GetSlice(bodyLen);

            // skip crc
            codec.ReadLong();

            return msg;
        }

        public BaseConsumerMessage Decode(string topic, IoBuffer buf, Type bodyType)
        {
            BaseConsumerMessage msg = new BaseConsumerMessage();

            PartialDecodedMessage decodedMessage = DecodePartial(buf);
            msg.Topic = topic;
            msg.RefKey = decodedMessage.Key;
            msg.BornTime = decodedMessage.BornTime;
            msg.RemainingRetries = decodedMessage.RemainingRetries;
            Dictionary<string, string> durableProperties = readProperties(decodedMessage.DurableProperties);
            Dictionary<string, string> volatileProperties = readProperties(decodedMessage.VolatileProperties);
            msg.PropertiesHolder = new PropertiesHolder(durableProperties, volatileProperties);
            IPayloadCodec bodyCodec = PayloadCodecFactory.GetCodecByType(decodedMessage.BodyCodecType);
            msg.Body = bodyCodec.Decode(decodedMessage.ReadBody(), bodyType);

            return msg;
        }

        public void Encode(PartialDecodedMessage msg, IoBuffer buf)
        {
            throw new NotImplementedException();
        }

        public byte[] Encode(ProducerMessage message, byte version)
        {
            var bodyCodec = PayloadCodecFactory.GetCodecByTopicName(message.Topic);
            var body = bodyCodec.Encode(message.Topic, message.GetBody<object>());
            var buf = IoBuffer.Allocate(body.Length + 150);
            buf.AutoExpand = true;
            Magic.WriteMagic(buf);
            buf.Put(version);
            Encode(message, buf, body, bodyCodec.Type);

            return null;
        }

        #endregion

        private void Encode(ProducerMessage message, IoBuffer buf, byte[] body, string codecType)
        {
            var codec = new HermesPrimitiveCodec(buf);
            var indexBeginning = buf.Position;
            codec.WriteInt(-1); // placeholder for whole length
            var indexAfterWholeLen = buf.Position;
            codec.WriteInt(-1); // placeholder for header length
            codec.WriteInt(-1); // placeholder for body length
            var indexBeforeHeader = buf.Position;

            // header begin
            codec.WriteString(message.Key);
            codec.WriteLong(message.BornTime);
            codec.WriteInt(0); //remaining retries
            codec.WriteString(codecType);

            var propertiesHolder = message.PropertiesHolder;
            WriteProperties(propertiesHolder.DurableProperties, buf, codec);
            WriteProperties(propertiesHolder.VolatileProperties, buf, codec);
            // header end

            var headerLen = buf.Position - indexBeforeHeader;

            //body begin
            var indexBeforeBody = buf.Position;
            buf.Put(body);
            var bodyLen = buf.Position - indexBeforeBody;
            //body end

            //crc
            codec.WriteLong(ChecksumUtil.Crc32(buf.GetSlice(indexBeforeHeader, headerLen + bodyLen)));
            var indexEnd = buf.Position;

            var wholeLen = indexEnd - indexAfterWholeLen;

            // refill whole length
            buf.Position = indexBeginning;
            codec.WriteInt(wholeLen);

            // refill header length
            codec.WriteInt(headerLen);

            // refill body length
            codec.WriteInt(bodyLen);

            buf.Position = indexEnd;
        }

        private void WriteProperties(Dictionary<string, string> properties, IoBuffer buf, HermesPrimitiveCodec codec)
        {
            var writeIndexBeforeLength = buf.Position;
            codec.WriteInt(-1);
            var writeIndexBeforeMap = buf.Position;
            codec.WriteStringStringMap(properties);
            var mapLength = buf.Position - writeIndexBeforeMap;
            var writeIndexEnd = buf.Position;
            buf.Position = writeIndexBeforeLength;
            codec.WriteInt(mapLength);
            buf.Position = writeIndexEnd;
        }

        private Dictionary<string, string> readProperties(IoBuffer buf)
        {
            HermesPrimitiveCodec codec = new HermesPrimitiveCodec(buf);
            return codec.ReadStringStringMap();
        }

        private void verifyChecksum(IoBuffer buf, int len)
        {
            // TODO 
        }
    }
}
