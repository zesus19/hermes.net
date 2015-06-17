using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arch.CMessaging.Client.Core.Pipeline.spi;

namespace Arch.CMessaging.Client.Core.Pipeline
{
    public class DefaultPipelineContext<T> : IPipelineContext
    {
        private IPipelineSink<T> sink;
        private int index;
        private IList<IValve> valves;
        private object source;
        private Dictionary<string, object> attrs;
        private T result;

        public DefaultPipelineContext(IList<IValve> valves, IPipelineSink<T> sink)
        {
            this.valves = valves;
            this.sink = sink;
            this.attrs = new Dictionary<string, object>();
        }

        public void Next(object payload)
        {
            if (index == 0) source = payload;
            if (index < valves.Count)
            {
                var valve = valves[index++];
                valve.Handle(this, payload);
            }
            else result = sink.Handle(this, payload);
        }

        object IPipelineContext.GetSource()
        {
            return source;
        }

        object IPipelineContext.Get(string name)
        {
            return attrs[name];
        }

        object IPipelineContext.GetResult()
        {
            return result;
        }

        public V GetSource<V>() { return (V)((IPipelineContext)this).GetSource(); }

        public void Put(string name, object value) 
        {
            attrs[name] = value;
        }

        public V Get<V>(string name) { return (V)((IPipelineContext)(this)).Get(name); }

        public T GetResult() { return result; }
    }
}
