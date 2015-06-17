using System;
using System.Diagnostics;
using Arch.CMessaging.Client.Net.Core.Buffer;
using Arch.CMessaging.Client.Net.Core.Filterchain;
using Arch.CMessaging.Client.Net.Core.Session;
using Arch.CMessaging.Client.Net.Core.Write;

namespace Arch.CMessaging.Client.Net.Filter.ErrorGenerating
{
    public class ErrorGeneratingFilter : IoFilterAdapter
    {
        private Int32 _removeByteProbability;
        private Int32 _insertByteProbability;
        private Int32 _changeByteProbability;
        private Int32 _removePduProbability;
        private Int32 _duplicatePduProbability;
        private Int32 _resendPduLasterProbability;
        private Int32 _maxInsertByte = 10;
        private Boolean _manipulateWrites;
        private Boolean _manipulateReads;
        private Random _rng = new Random();

        
        public override void FilterWrite(INextFilter nextFilter, IoSession session, IWriteRequest writeRequest)
        {
            if (_manipulateWrites)
            {
                // manipulate bytes
                IoBuffer buf = writeRequest.Message as IoBuffer;
                if (buf != null)
                {
                    ManipulateIoBuffer(session, buf);
                    IoBuffer buffer = InsertBytesToNewIoBuffer(session, buf);
                    if (buffer != null)
                    {
                        writeRequest = new DefaultWriteRequest(buffer, writeRequest.Future);
                    }
                    // manipulate PDU
                }
                else
                {
                    if (_duplicatePduProbability > _rng.Next())
                    {
                        nextFilter.FilterWrite(session, writeRequest);
                    }

                    if (_resendPduLasterProbability > _rng.Next())
                    {
                        // store it somewhere and trigger a write execution for
                        // later
                        // TODO
                    }
                    if (_removePduProbability > _rng.Next())
                    {
                        return;
                    }
                }
            }

            base.FilterWrite(nextFilter, session, writeRequest);
        }

        public override void MessageReceived(INextFilter nextFilter, IoSession session, Object message)
        {
            if (_manipulateReads)
            {
                IoBuffer buf = message as IoBuffer;
                if (buf != null)
                {
                    // manipulate bytes
                    ManipulateIoBuffer(session, buf);
                    IoBuffer buffer = InsertBytesToNewIoBuffer(session, buf);
                    if (buffer != null)
                    {
                        message = buffer;
                    }
                }
                else
                {
                    // manipulate PDU
                    // TODO
                }
            }

            base.MessageReceived(nextFilter, session, message);
        }

        private IoBuffer InsertBytesToNewIoBuffer(IoSession session, IoBuffer buffer)
        {
            if (_insertByteProbability > _rng.Next(1000))
            {
                Debug.WriteLine(buffer.GetHexDump());

                // where to insert bytes ?
                int pos = _rng.Next(buffer.Remaining) - 1;

                // how many byte to insert ?
                int count = _rng.Next(_maxInsertByte - 1) + 1;

                IoBuffer newBuff = IoBuffer.Allocate(buffer.Remaining + count);
                for (int i = 0; i < pos; i++)
                    newBuff.Put(buffer.Get());
                for (int i = 0; i < count; i++)
                {
                    newBuff.Put((byte)(_rng.Next(256)));
                }
                while (buffer.Remaining > 0)
                {
                    newBuff.Put(buffer.Get());
                }
                newBuff.Flip();

                Debug.WriteLine("Inserted " + count + " bytes.");
                Debug.WriteLine(newBuff.GetHexDump());
                return newBuff;
            }
            return null;
        }

        private void ManipulateIoBuffer(IoSession session, IoBuffer buffer)
        {
            if ((buffer.Remaining > 0) && (_removeByteProbability > _rng.Next(1000)))
            {
                Debug.WriteLine(buffer.GetHexDump());

                // where to remove bytes ?
                int pos = _rng.Next(buffer.Remaining);
                // how many byte to remove ?
                int count = _rng.Next(buffer.Remaining - pos) + 1;
                if (count == buffer.Remaining)
                    count = buffer.Remaining - 1;

                IoBuffer newBuff = IoBuffer.Allocate(buffer.Remaining - count);
                for (int i = 0; i < pos; i++)
                    newBuff.Put(buffer.Get());

                buffer.Skip(count); // hole
                while (newBuff.Remaining > 0)
                    newBuff.Put(buffer.Get());
                newBuff.Flip();
                // copy the new buffer in the old one
                buffer.Rewind();
                buffer.Put(newBuff);
                buffer.Flip();

                Debug.WriteLine("Removed " + count + " bytes at position " + pos + ".");
                Debug.WriteLine(buffer.GetHexDump());
            }
            if ((buffer.Remaining > 0) && (_changeByteProbability > _rng.Next(1000)))
            {
                Debug.WriteLine(buffer.GetHexDump());

                // how many byte to change ?
                int count = _rng.Next(buffer.Remaining - 1) + 1;

                byte[] values = new byte[count];
                _rng.NextBytes(values);
                for (int i = 0; i < values.Length; i++)
                {
                    int pos = _rng.Next(buffer.Remaining);
                    buffer.Put(pos, values[i]);
                }

                Debug.WriteLine("Modified " + count + " bytes.");
                Debug.WriteLine(buffer.GetHexDump());
            }
        }

        public Int32 RemoveByteProbability
        {
            get { return _removeByteProbability; }
            set { _removeByteProbability = value; }
        }

        public Int32 InsertByteProbability
        {
            get { return _insertByteProbability; }
            set { _insertByteProbability = value; }
        }

        public Int32 ChangeByteProbability
        {
            get { return _changeByteProbability; }
            set { _changeByteProbability = value; }
        }

        public Int32 RemovePduProbability
        {
            get { return _removePduProbability; }
            set { _removePduProbability = value; }
        }

        public Int32 DuplicatePduProbability
        {
            get { return _duplicatePduProbability; }
            set { _duplicatePduProbability = value; }
        }

        public Int32 ResendPduLasterProbability
        {
            get { return _resendPduLasterProbability; }
            set { _resendPduLasterProbability = value; }
        }

        public Int32 MaxInsertByte
        {
            get { return _maxInsertByte; }
            set { _maxInsertByte = value; }
        }

        public Boolean ManipulateWrites
        {
            get { return _manipulateWrites; }
            set { _manipulateWrites = value; }
        }

        public Boolean ManipulateReads
        {
            get { return _manipulateReads; }
            set { _manipulateReads = value; }
        }
    }
}
