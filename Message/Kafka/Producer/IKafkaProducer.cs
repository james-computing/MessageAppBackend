namespace Message.Kafka.Producer
{
    public interface IKafkaProducer : IAsyncDisposable
    {
        public Task ProduceToKafkaAsync(string senderId, string receiverId, string message);
    }
}
