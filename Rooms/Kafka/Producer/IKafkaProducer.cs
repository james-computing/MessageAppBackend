using Rooms.Kafka.Keys;

namespace Rooms.Kafka.Producer
{
    public interface IKafkaProducer : IAsyncDisposable
    {
        public Task ProduceToKafkaAsync(Key key, string value);
    }
}