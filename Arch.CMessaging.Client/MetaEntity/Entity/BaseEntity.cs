using System;

namespace Arch.CMessaging.Client.MetaEntity.Entity
{
	public class BaseEntity
	{
		public BaseEntity ()
		{
		}

		protected new bool Equals(Object o1, Object o2) {
			if (o1 == null) {
				return o2 == null;
			} else if (o2 == null) {
				return false;
			} else {
				return o1.Equals(o2);
			}
		}
	}
}

