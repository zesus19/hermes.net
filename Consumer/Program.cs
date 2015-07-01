using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arch.CMessaging.Client.Core.Message;
using Arch.CMessaging.Client.Net.Core.Buffer;
using Arch.CMessaging.Client.Net.Filter.Codec;
using Arch.CMessaging.Client.Net.Transport.Socket;
using Arch.CMessaging.Client.Transport;
using Arch.CMessaging.Client.Transport.Command;
using Arch.CMessaging.Client.Core.Utils;
using Arch.CMessaging.Client.Net;
using Arch.CMessaging.Client.Net.Core.Session;
using Arch.CMessaging.Client.Transport.EndPoint;
using Arch.CMessaging.Client.Core.Ioc;

namespace Consumer
{
    class Program
    {
        static VenusContainer c = new VenusContainer();
        static void Main(string[] args)
        {
        //    c.Define<A1>();
        //    c.Define<B1>();
        //    var a = c.Lookup<A1>();
        //    var b = c.Lookup<A1>();

            var dddd = new OperationCanceledException() is Exception;

            var c = new VenusContainer();
            c.Define<A>();
            c.Define<B>();
            c.Define<C>();
            c.Define<D>();
            var b = c.Lookup<B>();

            b.Do();
            string s = string.Empty;

            //var c = new VenusContainer();
            //c.Define(typeof(S<BB<int>>), typeof(FF<BB2<int>>));
            //var map = c.LookupMap<S<BB<int>>>();
            var d = string.Empty;
        }

        #region example 1
        [Named]
        public abstract class A
        {
            [Inject]
            private D d;

            public void Do()
            {
                string s = string.Empty;
            }
            
        }

        [Named]
        public class B : A
        {
            [Inject]
            private C c;

            public void Do()
            {
                string s = string.Empty;
                base.Do();
            }
        }

        [Named]
        public class C
        {
 
        }

        [Named]
        public class D
        {

        }
        #endregion

        #region example2
        [Named]
        public class A1 : IInitializable
        {
            private IList<B1> list = new List<B1>();
            #region IInitializable Members

            public void Initialize()
            {
                try
                {
                    list.Add(c.Lookup<B1>());
                }
                catch (Exception ex)
                {
                    string s = string.Empty;
                }
            }

            #endregion
        }
        [Named]
        public class B1
        { 

        }
        #endregion

        #region example3
        public interface S<in T>
        {
            void Do();
        }

        public class FF<T> : S<T>
        {
            public void Do() { }
        }

        public interface BB<in T> { }

        public class BB2<T> : BB<T> { }

        #endregion
    }
}
