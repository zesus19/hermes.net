using System;
using Arch.CMessaging.Client.Net.Core.Buffer;

namespace Arch.CMessaging.Client.Net.Core.File
{
    public interface IFileRegion
    {
        String FullName { get; }
        Int64 Length { get; }
        Int64 Position { get; }
        Int64 RemainingBytes { get; }
        Int64 WrittenBytes { get; }
        Int32 Read(IoBuffer buffer);
        void Update(Int64 amount);
    }
}
