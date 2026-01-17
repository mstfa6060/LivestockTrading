namespace BaseModules.FileProvider.QueueWorker.Models;

public class KafkaConnectionOptions
{
    public string KafkaUrl { get; set; }
    public int KafkaPort { get; set; }
    public string ConsumerGroupName { get; set; }
    public string TopicName { get; set; }
}