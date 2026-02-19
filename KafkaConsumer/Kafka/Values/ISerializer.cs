namespace KafkaConsumer.Kafka.Values
{
    public interface ISerializer
    {
        public string Serialize<T>(T value);
        public T? Deserialize<T>(string value);
    }
}
