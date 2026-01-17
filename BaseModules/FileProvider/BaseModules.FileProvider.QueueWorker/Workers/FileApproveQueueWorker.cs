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

public class FileApproveQueueWorker : BackgroundService
{
    private readonly IOptions<KafkaConnectionOptions> _connectionOptions;
    private readonly IConsumer<Null, string> kafkaConsumer;

    private readonly ArfBlocksRequestOperator _requestOperator;

    public FileApproveQueueWorker(ArfBlocksDependencyProvider dependencyBuilder, IOptions<FileApproveQueueKafkaOptions> connectionOptions)
    {
        _requestOperator = new ArfBlocksRequestOperator(dependencyBuilder);
        this._connectionOptions = connectionOptions;
        this.kafkaConsumer = this.BuildKafkaConsumer();
    }

    public override void Dispose()
    {
        this.kafkaConsumer.Close(); // Commit offsets and leave the group cleanly.
        this.kafkaConsumer.Dispose();

        System.Console.WriteLine("Worker Background Service Stopped");

        base.Dispose();
    }

    public IConsumer<Null, string> BuildKafkaConsumer()
    {
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
        new Thread(async () => await StartConsumerLoop(cancellationToken)).Start();
        return Task.CompletedTask;
    }

    private async Task StartConsumerLoop(CancellationToken cancellationToken)
    {
        System.Console.WriteLine("File Approve Worker Background Service Started");

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
        // for: JSON Convert Errors
        // for: IO Operation Errors
        try
        {
            // Deserialize Data
            var fileApproveContract = JsonSerializer.Deserialize<FileApproveModelContract>(messageValue);

            // Create User Image
            var requestModel = new Application.EventHandlers.Files.Commands.Approve.RequestModel()
            {
                BucketId = fileApproveContract.BucketId,
                UserId = fileApproveContract.UserId,
            };
            var result = await _requestOperator.OperateEvent<Application.EventHandlers.Files.Commands.Approve.Handler>(requestModel);

            if (result.StatusCode != 200)
            {
                System.Console.WriteLine($"FILE APPROVE EVENT ERROR: {result.Error?.Message}");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"{ex.Message} :: {ex.InnerException?.Message}");
            return false;
        }
    }

}