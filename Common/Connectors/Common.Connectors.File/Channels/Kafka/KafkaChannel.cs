using System.Text.Json;
using Common.Contracts.Queue.Models;
using Confluent.Kafka;

namespace Common.Connectors.File.Channels;

public class KafkaChannel : IFileChannel
{
	private readonly KafkaChannelOptions _options;
	// private readonly IProducer<Null, string> _producer;
	private ProducerConfig config = null;

	public KafkaChannel(KafkaChannelOptions options)
	{
		_options = options;

		config = new ProducerConfig()
		{
			BootstrapServers = $"{_options.KafkaUrl}:{_options.KafkaPort}",
			EnableDeliveryReports = true,
			ConnectionsMaxIdleMs = 0,
		};

		// this._producer = new ProducerBuilder<Null, string>(config).Build();
	}

	public async Task<bool> Deliver(FileCreateModelContract fileData)
	{

		string serializedData = JsonSerializer.Serialize(fileData);
		// If serializers are not specified, default serializers from
		// `Confluent.Kafka.Serializers` will be automatically used where
		// available. Note: by default strings are encoded as UTF8.
		try
		{
			using (var producer = new ProducerBuilder<Null, string>(config).Build())
			{
				var message = new Message<Null, string> { Value = serializedData };
				var deliveryReport = await producer.ProduceAsync(_options.TopicName, message);

				if (deliveryReport.Status == PersistenceStatus.NotPersisted)
				{
					// It is common to write application logs to Kafka (note: this project does not provide
					// an example logger implementation that does this). Such an implementation should
					// ideally fall back to logging messages locally in the case of delivery problems.
					Console.WriteLine($"ERROR :::: Message delivery failed: {deliveryReport.Message.Value}");
				}
			}

			// Console.WriteLine($"Delivered '{deliveryReport.Value}' to '{deliveryReport.TopicPartitionOffset}'");
			return await Task.FromResult(true);
		}
		catch (ProduceException<Null, string> e)
		{
			Console.WriteLine($"######\n#####\n##### Delivery failed: {e.Error.Reason}");
			return await Task.FromResult(false);
		}
		catch (Exception exception)
		{
			Console.WriteLine($"$$$$$\n$$$$$\n$$$$$ Kafka Exception Throwed: {exception.Message}");
			return await Task.FromResult(false);
		}
	}

	public void Dispose()
	{
		// this._producer = null;
		System.Console.WriteLine("____________KAFKA PRODUCER DISPOSED_____________");
	}
}