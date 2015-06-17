using System;
using System.IO;

namespace Arch.CMessaging.Client.Net.Core.Buffer
{
    public class IoBufferStream : Stream
    {
        private readonly IoBuffer _buf;

        public IoBufferStream(IoBuffer buf)
        {
            _buf = buf;
        }
        public override Boolean CanRead
        {
            get { return true; }
        }

        public override Boolean CanSeek
        {
            get { return true; }
        }
        public override Boolean CanWrite
        {
            get { return true; }
        }
        public override void Flush()
        {
            // do nothing
        }

        public override Int64 Length
        {
            get { return _buf.Remaining; }
        }
        public override Int64 Position
        {
            get { return _buf.Position; }
            set { _buf.Position = (Int32)value; }
        }
        public override Int32 Read(Byte[] buffer, Int32 offset, Int32 count)
        {
            Int32 read = Math.Min(_buf.Remaining, count);
            _buf.Get(buffer, offset, read);
            return read;
        }
        public override Int64 Seek(Int64 offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    Position = offset;
                    break;
                case SeekOrigin.Current:
                    Position += offset;
                    break;
                case SeekOrigin.End:
                    Position = _buf.Remaining - offset;
                    break;
                default:
                    break;
            }
            return Position;
        }
        public override void SetLength(Int64 value)
        {
            throw new NotSupportedException();
        }
        public override void Write(Byte[] buffer, Int32 offset, Int32 count)
        {
            _buf.Put(buffer, offset, count);
        }
    }
}
