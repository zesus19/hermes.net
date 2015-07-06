using System;
using System.Collections.Generic;

namespace Arch.CMessaging.Client.Core.Message
{

    public class MessageStatus
    {
        public const string SUCCESS = "success";
        public const string FAIL = "fail";
        public const string NOT_SET = "set_set";
    }

    public interface IConsumerMessage
    {
        void nack();

        string GetProperty(string name);

        IEnumerator<string> GetPropertyNames();

        long BornTime{ get; }

        string Topic{ get; }

        string RefKey { get; }

        T GetBody<T>();

        string Status{ get; }

        void ack();
    }
}

