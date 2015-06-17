using System;

namespace Arch.CMessaging.Client.Net.Core.Buffer
{
    public interface IoBufferAllocator
    {
        IoBuffer Allocate(Int32 capacity);
        IoBuffer Wrap(Byte[] array);
        IoBuffer Wrap(Byte[] array, Int32 offset, Int32 length);
    }
}
