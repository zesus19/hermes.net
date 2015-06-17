using System;
using System.Collections.Generic;
using Arch.CMessaging.Client.Net.Core.Buffer;
using System.Diagnostics;

namespace Arch.CMessaging.Client.Net.Filter.Codec.StateMachine
{
    public abstract class DecodingStateMachine : IDecodingState
    {
        private readonly List<Object> _childProducts = new List<Object>();
        private readonly ChildOutput _childOutput;
        private IDecodingState _currentState;
        private Boolean _initialized;

        protected DecodingStateMachine()
        {
            _childOutput = new ChildOutput(this);
        }

        public IDecodingState Decode(IoBuffer input, IProtocolDecoderOutput output)
        {
            IDecodingState state = CurrentState;

            Int32 limit = input.Limit, pos = input.Position;

            try
            {
                while (true)
                {
                    // Wait for more data if all data is consumed.
                    if (pos == limit)
                        break;

                    IDecodingState oldState = state;
                    state = state.Decode(input, _childOutput);

                    // If finished, call finishDecode
                    if (state == null)
                        return FinishDecode(_childProducts, output);

                    Int32 newPos = input.Position;

                    // Wait for more data if nothing is consumed and state didn't change.
                    if (newPos == pos && oldState == state)
                        break;

                    pos = newPos;
                }

                return this;
            }
            catch (Exception)
            {
                state = null;
                throw;
            }
            finally
            {
                _currentState = state;

                // Destroy if decoding is finished or failed.
                if (state == null)
                    Cleanup();
            }
        }

        public IDecodingState FinishDecode(IProtocolDecoderOutput output)
        {
            IDecodingState nextState;
            IDecodingState state = CurrentState;

            try
            {
                while (true)
                {
                    IDecodingState oldState = state;
                    state = state.FinishDecode(_childOutput);
                    if (state == null)
                        // Finished
                        break;

                    // Exit if state didn't change.
                    if (oldState == state)
                        break;
                }
            }
            catch (Exception ex)
            {
                state = null;
                Debug.WriteLine("Ignoring the exception caused by a closed session. {0}", ex);
            }
            finally
            {
                _currentState = state;
                nextState = FinishDecode(_childProducts, output);
                if (state == null)
                    Cleanup();
            }
            return nextState;
        }

        protected abstract IDecodingState Init();
        protected abstract IDecodingState FinishDecode(List<Object> childProducts, IProtocolDecoderOutput output);
        protected abstract void Destroy();
        private IDecodingState CurrentState
        {
            get
            {
                IDecodingState state = _currentState;
                if (state == null)
                {
                    state = Init();
                    _initialized = true;
                }
                return state;
            }
        }

        private void Cleanup()
        {
            if (!_initialized)
                throw new InvalidOperationException();

            _initialized = false;
            _childProducts.Clear();
            try
            {
                Destroy();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to destroy a decoding state machine. {0}", ex);
            }
        }

        class ChildOutput : IProtocolDecoderOutput
        {
            private readonly DecodingStateMachine _parent;

            public ChildOutput(DecodingStateMachine parent)
            {
                _parent = parent;
            }

            public void Write(Object message)
            {
                _parent._childProducts.Add(message);
            }

            public void Flush(Core.Filterchain.INextFilter nextFilter, Core.Session.IoSession session)
            {
                // Do nothing
            }
        }
    }
}
