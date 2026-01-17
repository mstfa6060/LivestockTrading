using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Common.Contracts.Queue.Models;

using Microsoft.Extensions.Options;
using BaseModules.FileProvider.QueueWorker.Models;
using Arfware.ArfBlocks.Core;

namespace BaseModules.FileProvider.QueueWorker.Workers;

public class FileCreateQueueWorker : BackgroundService
{
	private readonly IOptions<KafkaConnectionOptions> _connectionOptions;
	private readonly IConsumer<Null, string> kafkaConsumer;

	private readonly ArfBlocksRequestOperator _requestOperator;

	public FileCreateQueueWorker(ArfBlocksDependencyProvider dependencyBuilder, IOptions<FileCreateQueueKafkaOptions> connectionOptions)
	{
		//System.Console.WriteLine("Part1");
		_requestOperator = new ArfBlocksRequestOperator(dependencyBuilder);
		this._connectionOptions = connectionOptions;
		this.kafkaConsumer = this.BuildKafkaConsumer();
	}

	public override void Dispose()
	{
		this.kafkaConsumer.Close(); // Commit offsets and leave the group cleanly.
		this.kafkaConsumer.Dispose();

		//System.Console.WriteLine("File Creator Worker Background Service Stopped");

		base.Dispose();
	}

	public IConsumer<Null, string> BuildKafkaConsumer()
	{
		//System.Console.WriteLine("Part2");

		var consumerConfig = new ConsumerConfig()
		{
			GroupId = _connectionOptions.Value.ConsumerGroupName,
			BootstrapServers = $"{_connectionOptions.Value.KafkaUrl}:{_connectionOptions.Value.KafkaPort}",
			// Note: The AutoOffsetReset property determines the start offset in the event
			// there are not yet any committed offsets for the consumer group for the
			// topic/partitions of interest. By default, offsets are committed automatically
			AutoOffsetReset = AutoOffsetReset.Earliest,
			AllowAutoCreateTopics = true,
			EnableAutoCommit = false
		};

		return new ConsumerBuilder<Null, string>(consumerConfig).Build();
	}

	protected override Task ExecuteAsync(CancellationToken cancellationToken)
	{
		//System.Console.WriteLine("Part3");

		new Thread(async () => await StartConsumerLoop(cancellationToken)).Start();
		return Task.CompletedTask;
	}

	private async Task StartConsumerLoop(CancellationToken cancellationToken)
	{
		//System.Console.WriteLine("Part4");

		//System.Console.WriteLine("File Creator Worker Background Service Started");

		kafkaConsumer.Subscribe(_connectionOptions.Value.TopicName);

		while (!cancellationToken.IsCancellationRequested)
		{
			try
			{
				var consumeResult = this.kafkaConsumer.Consume(cancellationToken);

				var isSuccedded = await CreateFile(consumeResult.Message.Value);

				if (isSuccedded)
					this.kafkaConsumer.Commit(consumeResult);
				else
					continue;
			}
			catch (OperationCanceledException)
			{
				break;
			}
			catch (ConsumeException e)
			{
				Console.WriteLine($"Unexpected error: {e}");
				// Consumer errors should generally be ignored (or logged) unless fatal.

				if (e.Error.IsFatal)
				{

					// https://github.com/edenhill/librdkafka/blob/master/INTRODUCTION.md#fatal-consumer-errors
				}
			}
			catch (Exception e)
			{
				Console.WriteLine($"Unexpected error: {e}");
				break;
			}
		}
	}

	public async Task<bool> CreateFile(string messageValue)
	{
		//System.Console.WriteLine("Part5");

		// for: JSON Convert Errors
		// for: IO Operation Errors
		try
		{
			//System.Console.WriteLine(messageValue);
			// Deserialize Data
			var fileCreateContract = JsonSerializer.Deserialize<FileCreateModelContract>(messageValue);

			//System.Console.WriteLine("====================");
			//System.Console.WriteLine($"File Create Contrack Type: {fileCreateContract.Type}");
			//System.Console.WriteLine("====================");

			if (fileCreateContract.Type == FileModelTypes.UserImage)
			{
				// Create User Image
				var requestModel = new Application.EventHandlers.UserImages.Commands.CreateOrReplace.RequestModel()
				{
					CompanyId = fileCreateContract.CompanyId,
					Name = fileCreateContract.FileName,
					Extention = fileCreateContract.Extention,
					Data = fileCreateContract.Data,
				};

				await _requestOperator.OperateEvent<Application.EventHandlers.UserImages.Commands.CreateOrReplace.Handler>(requestModel);
			}
			else if (fileCreateContract.Type == FileModelTypes.GroupImage)
			{
				// Create User Image
				var requestModel = new Application.EventHandlers.GroupImages.Commands.CreateOrReplace.RequestModel()
				{
					CompanyId = fileCreateContract.CompanyId,
					Name = fileCreateContract.FileName,
					Extention = fileCreateContract.Extention,
					Data = fileCreateContract.Data,
				};
				await _requestOperator.OperateEvent<Application.EventHandlers.GroupImages.Commands.CreateOrReplace.Handler>(requestModel);
			}
			else
			{
				//System.Console.WriteLine("::::ERROR::::");
				//System.Console.WriteLine("fileCreateContract.Type not handled");
			}

			return await Task.FromResult(true);
		}
		catch (Exception ex)
		{
			//System.Console.WriteLine($"{ex.Message} :: {ex.InnerException?.Message}");
			return await Task.FromResult(false);
		}
	}

}