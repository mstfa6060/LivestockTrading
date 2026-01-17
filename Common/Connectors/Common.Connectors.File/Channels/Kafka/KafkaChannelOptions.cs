namespace Common.Connectors.File.Channels;

public class KafkaChannelOptions
{
    public string KafkaUrl { get; set; }
    public int KafkaPort { get; set; }
    public string TopicName { get; set; }
}