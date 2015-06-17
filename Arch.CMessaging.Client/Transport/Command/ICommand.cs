using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arch.CMessaging.Client.Net.Core.Buffer;

namespace Arch.CMessaging.Client.Transport.Command
{
    public interface ICommand
    {
        Header Header { get; }

        void Parse(IoBuffer buf, Header header);

        void ToBytes(IoBuffer buf);

        void Release();
    }
}
