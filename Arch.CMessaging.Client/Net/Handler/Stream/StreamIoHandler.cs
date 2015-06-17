using System;
using System.IO;
using Arch.CMessaging.Client.Net.Core.Buffer;
using Arch.CMessaging.Client.Net.Core.Service;
using Arch.CMessaging.Client.Net.Core.Session;
using System.Diagnostics;

namespace Arch.CMessaging.Client.Net.Handler.Stream
{
    public abstract class StreamIoHandler : IoHandlerAdapter
    {
        private static readonly AttributeKey KEY_IN = new AttributeKey(typeof(StreamIoHandler), "in");
        private static readonly AttributeKey KEY_OUT = new AttributeKey(typeof(StreamIoHandler), "out");
        private Int32 _readTimeout;
        private Int32 _writeTimeout;

        public Int32 ReadTimeout
        {
            get { return _readTimeout; }
            set { _readTimeout = value; }
        }

        public Int32 WriteTimeout
        {
            get { return _writeTimeout; }
            set { _writeTimeout = value; }
        }
        
        public override void SessionOpened(IoSession session)
        {
            // Set timeouts
            session.Config.WriteTimeout = _writeTimeout;
            session.Config.SetIdleTime(IdleStatus.ReaderIdle, _readTimeout);

            // Create streams
            IoSessionStream input = new IoSessionStream();
            IoSessionStream output = new IoSessionStream(session);
            session.SetAttribute(KEY_IN, input);
            session.SetAttribute(KEY_OUT, output);
            ProcessStreamIo(session, input, output);
        }
        
        public override void SessionClosed(IoSession session)
        {
            IoSessionStream input = session.GetAttribute<IoSessionStream>(KEY_IN);
            IoSessionStream output = session.GetAttribute<IoSessionStream>(KEY_OUT);
            try
            {
                input.Close();
            }
            finally
            {
                output.Close();
            }
        }
        
        public override void MessageReceived(IoSession session, Object message)
        {
            IoSessionStream input = session.GetAttribute<IoSessionStream>(KEY_IN);
            input.Write((IoBuffer)message);
        }
       
        public override void ExceptionCaught(IoSession session, Exception cause)
        {
            IOException ioe = cause as IOException;
            if (ioe != null)
            {
                IoSessionStream input = session.GetAttribute<IoSessionStream>(KEY_IN);
                if (input != null)
                {
                    input.Exception = ioe;
                    return;
                }
            }
            Debug.WriteLine("Unexpected exception. {0}", cause);
            session.Close(true);
        }
        
        public override void SessionIdle(IoSession session, IdleStatus status)
        {
            if (status == IdleStatus.ReaderIdle)
                throw new IOException("Read timeout");
        }

        protected abstract void ProcessStreamIo(IoSession session, System.IO.Stream input, System.IO.Stream output);
    }
}
