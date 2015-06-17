using System;

namespace Arch.CMessaging.Client.Net.Core.Buffer
{

    public abstract class Buffer
    {
        private Int32 _mark = -1;
        private Int32 _position = 0;
        private Int32 _limit;
        private Int32 _capacity;

        protected Buffer(Int32 mark, Int32 pos, Int32 lim, Int32 cap)
        {
            if (cap < 0)
                throw new ArgumentException("Capacity should be >= 0", "cap");
            _capacity = cap;
            Limit = lim;
            Position = pos;
            if (mark >= 0)
            {
                if (mark > pos)
                    throw new ArgumentException("Invalid mark position", "mark");
                _mark = mark;
            }
        }

        public Int32 Capacity
        {
            get { return _capacity; }
        }

        public Int32 Position
        {
            get { return _position; }
            set
            {
                if ((value > _limit) || (value < 0))
                    throw new ArgumentException("Invalid position", "value");
                _position = value;
                if (_mark > _position) _mark = -1;
            }
        }

        public Int32 Limit
        {
            get { return _limit; }
            set
            {
                if ((value > _capacity) || (value < 0))
                    throw new ArgumentException("Invalid limit", "value");
                _limit = value;
                if (_position > _limit) _position = _limit;
                if (_mark > _limit) _mark = -1;
            }
        }

        public Int32 Remaining
        {
            get { return _limit - _position; }
        }

        public Boolean HasRemaining
        {
            get { return _position < _limit; }
        }

        public abstract Boolean ReadOnly { get; }

        public Buffer Mark()
        {
            _mark = _position;
            return this;
        }

        public Buffer Reset()
        {
            Int32 m = _mark;
            if (m < 0)
                throw new InvalidOperationException();
            _position = m;
            return this;
        }

        public Buffer Clear()
        {
            _position = 0;
            _limit = _capacity;
            _mark = -1;
            return this;
        }

        public Buffer Flip()
        {
            _limit = _position;
            _position = 0;
            _mark = -1;
            return this;
        }
        public Buffer Rewind()
        {
            _position = 0;
            _mark = -1;
            return this;
        }

        protected Int32 MarkValue
        {
            get { return _mark; }
            set { _mark = value; }
        }

        protected void Recapacity(Int32 capacity)
        {
            _capacity = capacity;
        }

        protected Int32 CheckIndex(Int32 i)
        {
            if ((i < 0) || (i >= _limit))
                throw new IndexOutOfRangeException();
            return i;
        }

        protected Int32 CheckIndex(Int32 i, Int32 nb)
        {
            if ((i < 0) || (nb > _limit - i))
                throw new IndexOutOfRangeException();
            return i;
        }

      
        protected Int32 NextGetIndex()
        {
            if (_position >= _limit)
                throw new BufferUnderflowException();
            return _position++;
        }

 
        protected Int32 NextGetIndex(Int32 nb)
        {
            if (_limit - _position < nb)
                throw new BufferUnderflowException();
            Int32 p = _position;
            _position += nb;
            return p;
        }

        protected Int32 NextPutIndex()
        {
            if (_position >= _limit)
                throw new OverflowException();
            return _position++;
        }

       
        protected Int32 NextPutIndex(Int32 nb)
        {
            if (_limit - _position < nb)
                throw new OverflowException();
            Int32 p = _position;
            _position += nb;
            return p;
        }

      
        protected static void CheckBounds(Int32 off, Int32 len, Int32 size)
        {
            if ((off | len | (off + len) | (size - (off + len))) < 0)
                throw new IndexOutOfRangeException();
        }
    }

   
    public enum ByteOrder
    {
        BigEndian,
        LittleEndian
    }
}
