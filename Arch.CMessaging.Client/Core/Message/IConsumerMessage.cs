using System;
using System.Collections.Generic;

namespace Arch.CMessaging.Client.Core.Message
{

	public enum MessageStatus
	{
		SUCCESS,
		FAIL,
		NOT_SET
	}

	public interface IConsumerMessage<T>
	{
		void nack ();

		String getProperty (String name);

		IEnumerable<String> getPropertyNames ();

		long getBornTime ();

		String getTopic ();

		String getRefKey ();

		T getBody ();

		MessageStatus getStatus ();

		void ack ();
	}
}

