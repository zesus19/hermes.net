using System;
using System.Collections.Concurrent;
using Arch.CMessaging.Client.Net.Core.Buffer;
using Arch.CMessaging.Client.Net.Core.Filterchain;
using Arch.CMessaging.Client.Net.Core.Session;
using Arch.CMessaging.Client.Net.Core.Write;
using System.Diagnostics;

namespace Arch.CMessaging.Client.Net.Filter.Buffer
{
    public class BufferedWriteFilter : IoFilterAdapter
    {
        public const Int32 DefaultBufferSize = 8192;
        private Int32 _bufferSize;
        private ConcurrentDictionary<IoSession, Lazy<IoBuffer>> _buffersMap;

        public BufferedWriteFilter()
            : this(DefaultBufferSize, null)
        { }

        public BufferedWriteFilter(Int32 bufferSize)
            : this(bufferSize, null)
        { }

        public BufferedWriteFilter(Int32 bufferSize, ConcurrentDictionary<IoSession, Lazy<IoBuffer>> buffersMap)
        {
            _bufferSize = bufferSize;
            _buffersMap = buffersMap == null ?
                new ConcurrentDictionary<IoSession, Lazy<IoBuffer>>() : buffersMap;
        }

        public Int32 BufferSize
        {
            get { return _bufferSize; }
            set { _bufferSize = value; }
        }

        
        public override void FilterWrite(INextFilter nextFilter, IoSession session, IWriteRequest writeRequest)
        {
            IoBuffer buf = writeRequest.Message as IoBuffer;
            if (buf == null)
                throw new ArgumentException("This filter should only buffer IoBuffer objects");
            else
                Write(session, buf);
        }

        public override void SessionClosed(INextFilter nextFilter, IoSession session)
        {
            Free(session);
            base.SessionClosed(nextFilter, session);
        }

        public override void ExceptionCaught(INextFilter nextFilter, IoSession session, Exception cause)
        {
            Free(session);
            base.ExceptionCaught(nextFilter, session, cause);
        }

        public void Flush(IoSession session)
        {
            Lazy<IoBuffer> lazy;
            _buffersMap.TryGetValue(session, out lazy);
            try
            {
                InternalFlush(session.FilterChain.GetNextFilter(this), session, lazy.Value);
            }
            catch (Exception e)
            {
                session.FilterChain.FireExceptionCaught(e);
            }
        }

        private void Write(IoSession session, IoBuffer data)
        {
            Lazy<IoBuffer> dest = _buffersMap.GetOrAdd(session,
                new Lazy<IoBuffer>(() => IoBuffer.Allocate(_bufferSize)));
            Write(session, data, dest.Value);
        }

        private void Write(IoSession session, IoBuffer data, IoBuffer buf)
        {
            try
            {
                Int32 len = data.Remaining;
                if (len >= buf.Capacity)
                {
                    /*
                     * If the request length exceeds the size of the output buffer,
                     * flush the output buffer and then write the data directly.
                     */
                    INextFilter nextFilter = session.FilterChain.GetNextFilter(this);
                    InternalFlush(nextFilter, session, buf);
                    nextFilter.FilterWrite(session, new DefaultWriteRequest(data));
                    return;
                }
                if (len > (buf.Limit - buf.Position))
                {
                    InternalFlush(session.FilterChain.GetNextFilter(this), session, buf);
                }

                lock (buf)
                {
                    buf.Put(data);
                }
            }
            catch (Exception e)
            {
                session.FilterChain.FireExceptionCaught(e);
            }
        }

        private void InternalFlush(INextFilter nextFilter, IoSession session, IoBuffer buf)
        {
            IoBuffer tmp = null;
            lock (buf)
            {
                buf.Flip();
                tmp = buf.Duplicate();
                buf.Clear();
            }
            Debug.WriteLine("Flushing buffer: " + tmp);
            nextFilter.FilterWrite(session, new DefaultWriteRequest(tmp));
        }

        private void Free(IoSession session)
        {
            Lazy<IoBuffer> lazy;
            if (_buffersMap.TryRemove(session, out lazy))
            {
                lazy.Value.Free();
            }
        }
    }
}
