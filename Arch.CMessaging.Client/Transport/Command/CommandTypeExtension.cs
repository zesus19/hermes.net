using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arch.CMessaging.Client.Transport.Command
{
    public static class CommandTypeExtension
    {
        static Dictionary<int, KeyValuePair<CommandType, Type>> commandTypes = new Dictionary<int, KeyValuePair<CommandType, Type>>()
        {
            {101, new KeyValuePair<CommandType, Type>(CommandType.MessageSend, typeof(SendMessageCommand))},
            {201, new KeyValuePair<CommandType, Type>(CommandType.AckMessageSend, typeof(SendMessageAckCommand))},
            {301, new KeyValuePair<CommandType, Type>(CommandType.ResultMessageSend, typeof(SendMessageResultCommand))}
        };

        public static int ToInt(this CommandType? commandType)
        {
            if (commandType == null) return -1;
            return Convert.ToInt32(commandType);
        }

        public static CommandType? ToCommandType(this int type)
        {
            CommandType? commandType = null;
            if (commandTypes.ContainsKey(type)) commandType = commandTypes[type].Key;
            return commandType;
        }

        public static ICommand ToCommand(this CommandType? commandType)
        {
            ICommand command = null;
            var val = commandType.ToInt();
            if (commandTypes.ContainsKey(val)) command = Activator.CreateInstance(commandTypes[val].Value) as ICommand;
            return command;
        }
    }
}