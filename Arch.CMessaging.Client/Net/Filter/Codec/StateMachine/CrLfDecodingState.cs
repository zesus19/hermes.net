using System;
using Arch.CMessaging.Client.Net.Core.Buffer;

namespace Arch.CMessaging.Client.Net.Filter.Codec.StateMachine
{
    public abstract class CrLfDecodingState : IDecodingState
    {
        private static readonly Byte CR = 13;
        private static readonly Byte LF = 10;

        private Boolean _hasCR;

        public IDecodingState Decode(IoBuffer input, IProtocolDecoderOutput output)
        {
            Boolean found = false;
            Boolean finished = false;
            while (input.HasRemaining)
            {
                Byte b = input.Get();
                if (!_hasCR)
                {
                    if (b == CR)
                    {
                        _hasCR = true;
                    }
                    else
                    {
                        if (b == LF)
                        {
                            found = true;
                        }
                        else
                        {
                            input.Position = input.Position - 1;
                            found = false;
                        }
                        finished = true;
                        break;
                    }
                }
                else
                {
                    if (b == LF)
                    {
                        found = true;
                        finished = true;
                        break;
                    }

                    throw new ProtocolDecoderException("Expected LF after CR but was: " + (b & 0xff));
                }
            }

            if (finished)
            {
                _hasCR = false;
                return FinishDecode(found, output);
            }

            return this;
        }

        public IDecodingState FinishDecode(IProtocolDecoderOutput output)
        {
            return FinishDecode(false, output);
        }

        protected abstract IDecodingState FinishDecode(Boolean foundCRLF, IProtocolDecoderOutput output);
    }
}
