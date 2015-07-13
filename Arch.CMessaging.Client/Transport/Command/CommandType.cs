using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arch.CMessaging.Client.Transport.Command
{
    public class CommandTypeToCommand
    {
        public static KeyValuePair<CommandType, Type> FindCommandInfoByCode(int code)
        {
            KeyValuePair<CommandType, Type> result;
            bool found = commandTypes.TryGetValue(code, out result);

            if (!found)
            {
                throw new Exception(string.Format("Unknown command type {0}", code));
            }

            return result;
        }

        private static Dictionary<int, KeyValuePair<CommandType, Type>> commandTypes = new Dictionary<int, KeyValuePair<CommandType, Type>>()
        {
            { 101, new KeyValuePair<CommandType, Type>(CommandType.MessageSend, typeof(SendMessageCommand)) },
            { 102, new KeyValuePair<CommandType, Type>(CommandType.MessageAck, typeof(AckMessageCommand)) },
            { 103, new KeyValuePair<CommandType, Type>(CommandType.MessagePull, typeof(PullMessageCommand)) },
            { 201, new KeyValuePair<CommandType, Type>(CommandType.AckMessageSend, typeof(SendMessageAckCommand)) },
            { 301, new KeyValuePair<CommandType, Type>(CommandType.ResultMessageSend, typeof(SendMessageResultCommand)) },
            { 302, new KeyValuePair<CommandType, Type>(CommandType.ResultMessagePull, typeof(PullMessageResultCommand)) }
        };
    }

    public enum CommandType
    {
        Dummy = 0,
        MessageSend = 101,
        MessageAck = 102,
        MessagePull = 103,
        AckMessageSend = 201,
        ResultMessageSend = 301,
        ResultMessagePull = 302
    }
}
