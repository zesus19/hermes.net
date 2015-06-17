using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arch.CMessaging.Client.Net.Core.Filterchain;
using Arch.CMessaging.Client.Net.Core.Buffer;
using Arch.CMessaging.Client.Net.Core.Session;
using Arch.CMessaging.Client.Net.Core.Write;
using Arch.CMessaging.Client.Net.Filter.Codec;

namespace Arch.CMessaging.Client.Transport
{
    public class MagicNumberPrepender : IoFilterAdapter
    {
        public override void FilterWrite(INextFilter nextFilter, IoSession session, IWriteRequest writeRequest)
        {
            var buf = writeRequest.Message as IoBuffer;
            if (buf != null && buf.Remaining != 0)
            {
                var length = buf.Remaining + Magic.Length;
                if (length < 0)
                    throw new ArgumentOutOfRangeException("length", "length is less than zero");

                var @out = IoBuffer.Allocate(length);
                Magic.WriteMagic(@out);
                @out.Put(buf);
                @out.Flip();
                base.FilterWrite(
                    nextFilter, session, new ProtocolCodecFilter.EncodedWriteRequest(@out, null, writeRequest.Destination));
            }
            else
                base.FilterWrite(nextFilter, session, writeRequest);
        }
    }
}
