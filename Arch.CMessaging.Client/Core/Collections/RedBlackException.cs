using System;

namespace Arch.CMessaging.Client.Core.Collections
{
	public class RedBlackException : Exception
	{
		public RedBlackException()
        {
    	}
			
		public RedBlackException(string msg) : base(msg) 
        {
		}			
	}
}
