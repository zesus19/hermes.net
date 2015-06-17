using System;
using System.Text;

namespace Arch.CMessaging.Client.Net.Filter.Codec.TextLine
{
    public class LineDelimiter
    {
        public static readonly LineDelimiter Default = new LineDelimiter(Environment.NewLine);
        public static readonly LineDelimiter Auto = new LineDelimiter(String.Empty);
        public static readonly LineDelimiter CRLF = new LineDelimiter("\r\n");
        public static readonly LineDelimiter Unix = new LineDelimiter("\n");
        public static readonly LineDelimiter Windows = CRLF;
        public static readonly LineDelimiter Mac = new LineDelimiter("\r");
        public static readonly LineDelimiter NUL = new LineDelimiter("\0");
        private readonly String _value;
        public LineDelimiter(String value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            _value = value;
        }
        public String Value
        {
            get { return _value; }
        }

        public override Int32 GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override Boolean Equals(Object obj)
        {
            if (Object.ReferenceEquals(this, obj))
                return true;
            LineDelimiter that = obj as LineDelimiter;
            if (that == null)
                return false;
            else
                return this._value.Equals(that._value);
        }

        public override String ToString()
        {
            if (_value.Length == 0)
                return "delimiter: auto";
            else
            {
                StringBuilder buf = new StringBuilder();
                buf.Append("delimiter:");

                for (Int32 i = 0; i < _value.Length; i++)
                {
                    buf.Append(" 0x");
                    buf.AppendFormat("{0:X}", _value[i]);
                }

                return buf.ToString();
            }
        }
    }
}
