using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arch.CMessaging.Client.Net.Core.Buffer;

namespace Arch.CMessaging.Client.Transport
{
    public class Magic
    {
        private static readonly byte[] MAGIC = new byte[] { (byte)'h', (byte)'e', (byte)'m', (byte)'s' };

        public static void ReadAndCheckMagic(IoBuffer buf)
        {
            var magic = new byte[Magic.Length];
            buf.Get(magic, 0, magic.Length);
            if (!magic.SequenceEqual(MAGIC))
                throw new ArgumentException("Magic number mismatch");
        }

        public static void WriteMagic(IoBuffer buf)
        {
            buf.Put(MAGIC);
        }

        public static int Length { get { return MAGIC.Length; } }
    }
}
