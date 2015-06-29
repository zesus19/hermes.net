using System;

namespace Arch.CMessaging.Client.MetaEntity.Entity
{
	public interface IVisitor
	{
		void visitApp(App app);

		void visitCodec(Codec codec);

		void visitConsumerGroup(ConsumerGroup consumerGroup);

		void visitDatasource(DataSource datasource);

		void visitEndpoint(Endpoint endpoint);

		void visitMeta(Meta meta);

		void visitPartition(Partition partition);

		void visitProducer(Producer producer);

		void visitProperty(Property property);

		void visitServer(Server server);

		void visitStorage(Storage storage);

		void visitTopic(Topic topic);
	}
}

