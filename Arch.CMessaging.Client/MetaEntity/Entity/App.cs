using System;
using Arch.CMessaging.Client.MetaEntity;

namespace Arch.CMessaging.Client.MetaEntity.Entity
{
	public class App : BaseEntity
	{
		public  long ID { get; set; }

		public App ()
		{
		}

		public App (long id)
		{
			this.ID = id;
		}

		public void accept (IVisitor visitor)
		{
			visitor.visitApp (this);
		}


		public override bool Equals (Object obj)
		{
			if (obj is App) {
				App _o = (App)obj;

				if (!Equals (ID, _o.ID)) {
					return false;
				}

				return true;
			}

			return false;
		}

		public override int GetHashCode ()
		{
			int hash = 0;

			hash = hash * 31 + (ID == null ? 0 : ID.GetHashCode ());

			return hash;
		}
	}
}

