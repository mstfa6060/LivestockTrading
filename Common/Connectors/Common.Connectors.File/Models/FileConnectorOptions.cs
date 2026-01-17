namespace Common.Connectors.File.Models;

public class FileConnectorOptions
{
    public string KafkaUrl { get; set; }
    public int KafkaPort { get; set; }
    public string TopicName { get; set; }
}