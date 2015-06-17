using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arch.CMessaging.Client.Core.Message.Payload
{
    public interface IPayloadCodec
    {
        string Type { get; }
        byte[] Encode(string topic, object obj);
	    T Decode<T>(byte[] raw);
    }
}
