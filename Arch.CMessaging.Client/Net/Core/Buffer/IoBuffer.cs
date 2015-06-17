using System;
using System.Text;

namespace Arch.CMessaging.Client.Net.Core.Buffer
{
    public abstract class IoBuffer : Buffer
    {
        private static IoBufferAllocator allocator = ByteBufferAllocator.Instance;

        public static IoBufferAllocator Allocator
        {
            get { return allocator; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                if (allocator != null && allocator != value && allocator is IDisposable)
                    ((IDisposable)allocator).Dispose();
                allocator = value;
            }
        }

        public static IoBuffer Allocate(Int32 capacity)
        {
            return allocator.Allocate(capacity);
        }

        public static IoBuffer Wrap(Byte[] array)
        {
            return allocator.Wrap(array);
        }

        
        public static IoBuffer Wrap(Byte[] array, Int32 offset, Int32 length)
        {
            return allocator.Wrap(array, offset, length);
        }

        public static Int32 NormalizeCapacity(Int32 requestedCapacity)
        {
            if (requestedCapacity < 0)
                return Int32.MaxValue;

            Int32 newCapacity = HighestOneBit(requestedCapacity);
            newCapacity <<= (newCapacity < requestedCapacity ? 1 : 0);
            return newCapacity < 0 ? Int32.MaxValue : newCapacity;
        }

        private static Int32 HighestOneBit(Int32 i)
        {
            i |= (i >> 1);
            i |= (i >> 2);
            i |= (i >> 4);
            i |= (i >> 8);
            i |= (i >> 16);
            return i - (i >> 1);
        }

        protected IoBuffer(Int32 mark, Int32 pos, Int32 lim, Int32 cap)
            : base(mark, pos, lim, cap)
        { }

        public abstract IoBufferAllocator BufferAllocator { get; }

        public abstract ByteOrder Order { get; set; }

        public new virtual Int32 Capacity
        {
            get { return base.Capacity; }
            set { throw new NotSupportedException(); }
        }

        public new virtual Int32 Position
        {
            get { return base.Position; }
            set { base.Position = value; }
        }

        public new virtual Int32 Limit
        {
            get { return base.Limit; }
            set { base.Limit = value; }
        }

        public new virtual Int32 Remaining
        {
            get { return base.Remaining; }
        }

        public new virtual Boolean HasRemaining
        {
            get { return base.HasRemaining; }
        }

        public abstract Boolean AutoExpand { get; set; }

        public abstract Boolean AutoShrink { get; set; }

        public abstract Boolean Derived { get; }

        public abstract Int32 MinimumCapacity { get; set; }

        public abstract Boolean HasArray { get; }

        public new virtual IoBuffer Mark()
        {
            base.Mark();
            return this;
        }

        public new virtual IoBuffer Reset()
        {
            base.Reset();
            return this;
        }

        public new virtual IoBuffer Clear()
        {
            base.Clear();
            return this;
        }

        public new virtual IoBuffer Flip()
        {
            base.Flip();
            return this;
        }
        public new virtual IoBuffer Rewind()
        {
            base.Rewind();
            return this;
        }

        public abstract IoBuffer Expand(Int32 expectedRemaining);

        public abstract IoBuffer Expand(Int32 position, Int32 expectedRemaining);

        public abstract IoBuffer Shrink();

        public abstract IoBuffer Sweep();

        public abstract IoBuffer Sweep(Byte value);

        public abstract IoBuffer FillAndReset(Int32 size);

        public abstract IoBuffer FillAndReset(Byte value, Int32 size);

        public abstract IoBuffer Fill(Int32 size);

        public abstract IoBuffer Fill(Byte value, Int32 size);

        public abstract String GetHexDump();
        public abstract String GetHexDump(Int32 lengthLimit);

        public abstract Boolean PrefixedDataAvailable(Int32 prefixLength);

        public abstract Boolean PrefixedDataAvailable(Int32 prefixLength, Int32 maxDataLength);
        public abstract Int32 IndexOf(Byte b);

        public abstract String GetPrefixedString(Encoding encoding);

        public abstract String GetPrefixedString(Int32 prefixLength, Encoding encoding);

        public abstract IoBuffer PutPrefixedString(String value, Encoding encoding);
        public abstract IoBuffer PutPrefixedString(String value, Int32 prefixLength, Encoding encoding);

        public abstract Object GetObject();

        public abstract IoBuffer PutObject(Object o);
        public abstract Byte Get();
        public abstract Byte Get(Int32 index);
        public abstract IoBuffer Get(Byte[] dst, Int32 offset, Int32 length);
        public abstract ArraySegment<Byte> GetRemaining();
        public abstract void Free();
        public abstract IoBuffer Slice();
        public abstract IoBuffer GetSlice(Int32 index, Int32 length);
        public abstract IoBuffer GetSlice(Int32 length);
        public abstract IoBuffer Duplicate();
        public abstract IoBuffer AsReadOnlyBuffer();
        public abstract IoBuffer Skip(Int32 size);
        public abstract IoBuffer Put(Byte b);
        public abstract IoBuffer Put(Int32 i, Byte b);
        public abstract IoBuffer Put(Byte[] src, Int32 offset, Int32 length);
        public abstract IoBuffer Put(IoBuffer src);
        public abstract IoBuffer Compact();
        public abstract IoBuffer Put(Byte[] src);
        public abstract IoBuffer PutString(String s);
        public abstract IoBuffer PutString(String s, Encoding encoding);
        public abstract IoBuffer PutString(String s, Int32 fieldSize, Encoding encoding);
        public abstract String GetString(Encoding encoding);
        public abstract String GetString(Int32 fieldSize, Encoding encoding);
        public abstract Char GetChar();
        public abstract Char GetChar(Int32 index);
        public abstract IoBuffer PutChar(Char value);
        public abstract IoBuffer PutChar(Int32 index, Char value);
        public abstract Int16 GetInt16();
        public abstract Int16 GetInt16(Int32 index);
        public abstract IoBuffer PutInt16(Int16 value);
        public abstract IoBuffer PutInt16(Int32 index, Int16 value);
        public abstract Int32 GetInt32();
        public abstract Int32 GetInt32(Int32 index);
        public abstract IoBuffer PutInt32(Int32 value);
        public abstract IoBuffer PutInt32(Int32 index, Int32 value);
        public abstract Int64 GetInt64();
        public abstract Int64 GetInt64(Int32 index);
        public abstract IoBuffer PutInt64(Int64 value);
        public abstract IoBuffer PutInt64(Int32 index, Int64 value);
        public abstract Single GetSingle();
        public abstract Single GetSingle(Int32 index);
        public abstract IoBuffer PutSingle(Single value);
        public abstract IoBuffer PutSingle(Int32 index, Single value);
        public abstract Double GetDouble();
        public abstract Double GetDouble(Int32 index);
        public abstract IoBuffer PutDouble(Double value);
        public abstract IoBuffer PutDouble(Int32 index, Double value);
    }
}
