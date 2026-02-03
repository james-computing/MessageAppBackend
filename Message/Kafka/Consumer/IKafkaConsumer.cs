namespace Message.Kafka.Consumer
{
    public interface IKafkaConsumer : IAsyncDisposable
    {
        public Task ConsumeMessagesFromKafkaAsync(CancellationToken stoppingToken);
    }
}
