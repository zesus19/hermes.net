using System;

namespace Arch.CMessaging.Client.MetaEntity.Entity
{
	public class Server : BaseEntity
	{
		public String ID { get; set; }

		public String Host { get; set; }

		public int Port { get; set; }

		public Server ()
		{
		}

		public Server (String id)
		{
			this.ID = id;
		}

		public void accept (IVisitor visitor)
		{
			visitor.visitServer (this);
		}

		
		public override bool Equals (Object obj)
		{
			if (obj is Server) {
				Server _o = (Server)obj;

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

