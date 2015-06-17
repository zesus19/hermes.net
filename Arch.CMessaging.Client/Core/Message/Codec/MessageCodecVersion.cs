using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arch.CMessaging.Client.Core.Message.Codec.Internal;

namespace Arch.CMessaging.Client.Core.Message.Codec
{
    public abstract class MessageCodecVersion
    {
        static readonly Dictionary<byte, MessageCodecVersion> versions;
        protected MessageCodecVersion(byte version, IMessageCodecHandler handler)
        {
            this.Version = version;
            this.Handler = handler;
        }

        static MessageCodecVersion()
        {
            BINARY_V1 = new BIN_V1();
            versions = new Dictionary<byte, MessageCodecVersion>();
            versions[BINARY_V1.Version] = BINARY_V1;
        }
        public byte Version { get; private set; }
        public IMessageCodecHandler Handler { get; private set; }
        public static MessageCodecVersion BINARY_V1 { get; private set; }
        public static MessageCodecVersion ValueOf(byte val)
        {
            MessageCodecVersion version = null;
            versions.TryGetValue(val, out version);
            return version;
        }
    }

    public class BIN_V1 : MessageCodecVersion
    {
        public BIN_V1()
            : base((byte)1, new MessageCodecBinaryV1Handler()) { }
    }
}
