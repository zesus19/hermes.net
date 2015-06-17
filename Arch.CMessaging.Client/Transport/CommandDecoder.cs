using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arch.CMessaging.Client.Net.Filter.Codec.Demux;
using Arch.CMessaging.Client.Net.Core.Session;
using Arch.CMessaging.Client.Net.Core.Buffer;
using Arch.CMessaging.Client.Net.Filter.Codec;
using Arch.CMessaging.Client.Transport.Command.Parser;
using Freeway.Logging;

namespace Arch.CMessaging.Client.Transport
{
    public class CommandDecoder : IMessageDecoder
    {
        private const int MAX_FRAME_LENGTH = int.MaxValue;
        private const int MIN_REQUIRED_FRAME_LENGTH = 4 + 4; //Magic + Frame_Length
        private ICommandParser commandParser;
        private ILog log = LogManager.GetLogger(typeof(CommandDecoder));

        public CommandDecoder()
        {
            this.commandParser = new DefaultCommandParser();
        }

        #region IMessageDecoder Members

        public MessageDecoderResult Decodable(IoSession session, IoBuffer input)
        {
            if (input.Remaining < MIN_REQUIRED_FRAME_LENGTH)
                return MessageDecoderResult.NeedData;
            try
            {
                Magic.ReadAndCheckMagic(input);
            }
            catch (Exception) { return MessageDecoderResult.NotOK; }

            var frameLength = input.GetInt32();
            if (input.Remaining < frameLength)
                return MessageDecoderResult.NeedData;

            return MessageDecoderResult.OK;
        }

        public MessageDecoderResult Decode(IoSession session, IoBuffer input, IProtocolDecoderOutput output)
        {
            try
            {
                input.Skip(MIN_REQUIRED_FRAME_LENGTH);
                var command = commandParser.Parse(input);
                output.Write(command);
                return MessageDecoderResult.OK;
            }
            catch (Exception ex)
            {
                var remoteAddr = string.Empty;
                if (session != null && session.RemoteEndPoint != null) remoteAddr = session.RemoteEndPoint.ToString();
                log.Error(ex, new Dictionary<string, string> { { "RemoteAddr", remoteAddr } });
                return MessageDecoderResult.NotOK;
            }
        }

        public void FinishDecode(IoSession session, IProtocolDecoderOutput output)
        {
        }

        #endregion
    }
}
