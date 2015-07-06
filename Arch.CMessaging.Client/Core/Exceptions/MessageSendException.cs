using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arch.CMessaging.Client.Core.Exceptions
{
    public class MessageSendException : Exception
    {
        private const long serialVersionUID = 1L;
        public MessageSendException(string message) : base(message) { }
        public MessageSendException(string message, Exception ex) : base(message, ex) { }
    }
}
