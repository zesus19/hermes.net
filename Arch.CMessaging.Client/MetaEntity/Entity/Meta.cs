using System;
using System.Collections.Generic;
using Arch.CMessaging.Client.MetaEntity.Entity;
using Arch.CMessaging.Client.Core.Utils;

namespace Arch.CMessaging.Client.MetaEntity.Entity
{
	public class Meta : BaseEntity
	{
		public long Version { get; set; }

		public Dictionary<String, Topic> Topics { get; set; }

		public Dictionary<long, App> Apps { get; set; }

		public Dictionary<String, Codec> Codecs { get; set; }

		public Dictionary<String, Endpoint> Endpoints { get; set; }

		public Dictionary<String, Storage> Storages{ get; set; }

		public Dictionary<String, Server> Servers{ get; set; }

		public Meta ()
		{
			Topics = new Dictionary<String, Topic> ();
			Apps = new Dictionary<long, App> ();
			Codecs = new Dictionary<String, Codec> ();
			Endpoints = new Dictionary<String, Endpoint> ();
			Storages = new Dictionary<String, Storage> ();
			Servers = new Dictionary<String, Server> ();
		}

		
		public void accept (IVisitor visitor)
		{
			visitor.visitMeta (this);
		}

		public Meta addApp (App app)
		{
			Apps.Add (app.ID, app);
			return this;
		}

		public Meta addCodec (Codec codec)
		{
			Codecs.Add (codec.Type, codec);
			return this;
		}

		public Meta addEndpoint (Endpoint endpoint)
		{
			Endpoints.Add (endpoint.ID, endpoint);
			return this;
		}

		public Meta addServer (Server server)
		{
			Servers.Add (server.ID, server);
			return this;
		}

		public Meta addStorage (Storage storage)
		{
			Storages.Add (storage.Type, storage);
			return this;
		}

		public Meta addTopic (Topic topic)
		{
			Topics.Add (topic.Name, topic);
			return this;
		}

		
		public override bool Equals (Object obj)
		{
			if (obj is Meta) {
				Meta _o = (Meta)obj;

				if (!Equals (Version, _o.Version)) {
					return false;
				}

				if (!Equals (Topics, _o.Topics)) {
					return false;
				}

				if (!Equals (Apps, _o.Apps)) {
					return false;
				}

				if (!Equals (Codecs, _o.Codecs)) {
					return false;
				}

				if (!Equals (Endpoints, _o.Endpoints)) {
					return false;
				}

				if (!Equals (Storages, _o.Storages)) {
					return false;
				}

				if (!Equals (Servers, _o.Servers)) {
					return false;
				}


				return true;
			}

			return false;
		}

		public App FindApp (long id)
		{
			return CollectionUtil.TryGet (Apps, id);
		}

		public Codec FindCodec (String type)
		{
			return CollectionUtil.TryGet (Codecs, type);
		}

		public Endpoint FindEndpoint (String id)
		{
			return CollectionUtil.TryGet (Endpoints, id);
		}

		public Server FindServer (String id)
		{
			return CollectionUtil.TryGet (Servers, id);
		}

		public Storage FindStorage (String type)
		{
			return CollectionUtil.TryGet (Storages, type);
		}

		public Topic FindTopic (String name)
		{
			return CollectionUtil.TryGet (Topics, name);

		}

		public override int GetHashCode ()
		{
			int hash = 0;

			hash = hash * 31 + (Version == null ? 0 : Version.GetHashCode ());
			hash = hash * 31 + (Topics == null ? 0 : Topics.GetHashCode ());
			hash = hash * 31 + (Apps == null ? 0 : Apps.GetHashCode ());
			hash = hash * 31 + (Codecs == null ? 0 : Codecs.GetHashCode ());
			hash = hash * 31 + (Endpoints == null ? 0 : Endpoints.GetHashCode ());
			hash = hash * 31 + (Storages == null ? 0 : Storages.GetHashCode ());
			hash = hash * 31 + (Servers == null ? 0 : Servers.GetHashCode ());

			return hash;
		}

		
		public void mergeAttributes (Meta other)
		{
			if (other.Version != null) {
				Version = other.Version;
			}
		}

		public bool RemoveApp (long id)
		{
			return Apps.Remove (id);
		}

		public bool RemoveCodec (String type)
		{
			return Codecs.Remove (type);
		}

		public bool RemoveEndpoint (String id)
		{
			return Endpoints.Remove (id);
		}

		public bool RemoveServer (String id)
		{
			return Servers.Remove (id);
		}

		public bool RemoveStorage (String type)
		{
			return Storages.Remove (type);
		}

		public bool RemoveTopic (String name)
		{
			return Topics.Remove (name);
		}
			
	}
}

