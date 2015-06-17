using System;
using Arch.CMessaging.Client.Net.Core.Buffer;
using Arch.CMessaging.Client.Net.Core.File;

namespace Arch.CMessaging.Client.Net.Filter.Stream
{
    public class FileRegionWriteFilter : AbstractStreamWriteFilter<IFileRegion>
    {
        protected override IoBuffer GetNextBuffer(IFileRegion fileRegion)
        {
            if (fileRegion.RemainingBytes <= 0L)
                return null;

            Int32 bufferSize = (Int32)Math.Min(WriteBufferSize, fileRegion.RemainingBytes);
            IoBuffer buffer = IoBuffer.Allocate(bufferSize);
            fileRegion.Read(buffer);
            buffer.Flip();
            return buffer;
        }
    }
}
