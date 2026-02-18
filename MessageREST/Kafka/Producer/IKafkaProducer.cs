using MessageREST.Kafka.Keys;

namespace MessageREST.Kafka.Producer
{
    public interface IKafkaProducer : IAsyncDisposable
    {
        public Task ProduceToKafkaAsync(Key key, string value);
    }
}