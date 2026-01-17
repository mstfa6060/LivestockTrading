using Common.Contracts.Queue.Models;
using Common.Connectors.File.Channels;
using Common.Connectors.File.Models;

namespace Common.Connectors.File;

public class FileConnector
{
	private readonly KafkaChannel kafkaChannel;
	private readonly WorkerChannel workerChannel;

	public FileConnector(FileConnectorOptions options)
	{
		this.kafkaChannel = new KafkaChannel(new KafkaChannelOptions
		{
			KafkaUrl = options.KafkaUrl,
			KafkaPort = options.KafkaPort,
			TopicName = options.TopicName,
		});

		this.workerChannel = new WorkerChannel();
	}

	public async Task DeliverUserImage(Guid companyId, string name, string extention, byte[] data)
	{
		var fileData = new FileCreateModelContract()
		{
			UniqueId = Guid.NewGuid(),
			CompanyId = companyId,
			FileName = name,
			Extention = extention,
			Data = data,
			Type = FileModelTypes.UserImage,
		};

		// Try deliver email data to Kafka
		bool isDeliveredToKafka = await this.kafkaChannel.Deliver(fileData);
		if (isDeliveredToKafka)
			return;

		System.Console.WriteLine("Error: File data could not be delivered to Kafka !");

		// Try deliver email data to Mail Worker
		bool isDeliveredToWorker = await this.workerChannel.Deliver(fileData);
		if (isDeliveredToWorker)
			return;

		// Error Handling will be here...

		System.Console.WriteLine("Error: File data could not be delivered to File Worker !");
	}

	public async Task DeliverGroupImage(Guid companyId, string name, string extention, byte[] data)
	{
		var fileData = new FileCreateModelContract()
		{
			UniqueId = Guid.NewGuid(),
			CompanyId = companyId,
			FileName = name,
			Extention = extention,
			Data = data,
			Type = FileModelTypes.GroupImage,
		};

		// Try deliver email data to Kafka
		bool isDeliveredToKafka = await this.kafkaChannel.Deliver(fileData);
		if (isDeliveredToKafka)
			return;

		System.Console.WriteLine("Error: File data could not be delivered to Kafka !");

		// Try deliver email data to Mail Worker
		bool isDeliveredToWorker = await this.workerChannel.Deliver(fileData);
		if (isDeliveredToWorker)
			return;

		// Error Handling will be here...

		System.Console.WriteLine("Error: File data could not be delivered to File Worker !");
	}
}