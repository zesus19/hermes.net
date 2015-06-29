using System;
using Arch.CMessaging.Client.Net.Core.Buffer;

namespace Arch.CMessaging.Client.Transport
{
	public interface ITransferCallback
	{
		void transfer (IoBuffer buf);
	}

}
