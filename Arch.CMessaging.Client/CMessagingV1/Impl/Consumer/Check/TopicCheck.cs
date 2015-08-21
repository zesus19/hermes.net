using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Arch.CMessaging.Client.Impl.Consumer.Check
{
    public class TopicCheck
    {
        public static void Check(string topic)
        {
            if (string.IsNullOrWhiteSpace(topic))
            {
                throw new Exception("Topic不能为空.");
            }
            if (topic.Length > 80)
            {
                throw new Exception("Topic长度不能超过80字符.");
            }
            var topics = topic.Split(',');
            if (topics.Length > 5)
            {
                throw new Exception("同时最多支持订阅五个Topic");
            }
            var r = new Regex("^[.#*,a-zA-Z0-9]+$");
            var m = r.Match(topic);
            if (!m.Success)
            {
                throw new Exception("Topic只能使用数字，大写和小写英文字母，点号，星号，井号.");
            }
        }
    }
}
