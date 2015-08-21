using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arch.CMessaging.Client.API;
using Arch.CMessaging.Client.Core.Message;

namespace Arch.CMessaging.Client.Impl.Consumer
{
    public class HermesMessage : IMessage
    {    
        private HermesMessageReader messageReader;
        
        public HermesMessage(HermesMessageReader messageReader)
        {
            this.messageReader = messageReader;
        }

        #region IMessage Members

        public CMessaging.Core.Content.IHeaderProperties HeaderProperties
        {
            get
            {
                return messageReader.HeaderProperties;
            }
        }

        private string text;
        public string GetText()
        {
            if (text == null)
                text = messageReader.GetText();
            return text;
        }

        private byte[] binary;
        public byte[] GetBinary()
        {
            if (binary == null)
                binary = messageReader.GetBinary();
            return binary;
        }

        private object messageVal;
        public TObject GetObject<TObject>()
        {
            if (messageVal == null)
            {
                messageVal = messageReader.GetObject<TObject>();
            }
            return (TObject)messageVal;
        }

        public System.IO.Stream GetStream()
        {
            return messageReader.GetStream();
        }

        public CMessaging.Core.Content.AckMode Acks
        {
            get;
            set;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (Acks == CMessaging.Core.Content.AckMode.Ack) messageReader.RawMessage.Ack();
            else messageReader.RawMessage.Nack();
            messageReader.Dispose();
        }

        #endregion
    }
}
