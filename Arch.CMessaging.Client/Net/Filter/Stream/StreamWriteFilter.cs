using System;
using Arch.CMessaging.Client.Net.Core.Buffer;

namespace Arch.CMessaging.Client.Net.Filter.Stream
{
    public class StreamWriteFilter : AbstractStreamWriteFilter<System.IO.Stream>
    {
        protected override IoBuffer GetNextBuffer(System.IO.Stream stream)
        {
            Byte[] bytes = new Byte[WriteBufferSize];

            Int32 off = 0;
            Int32 n = 0;
            while (off < bytes.Length && (n = stream.Read(bytes, off, bytes.Length - off)) > 0)
            {
                off += n;
            }

            if (n <= 0 && off == 0)
                return null;

            return IoBuffer.Wrap(bytes, 0, off);
        }
    }
}
