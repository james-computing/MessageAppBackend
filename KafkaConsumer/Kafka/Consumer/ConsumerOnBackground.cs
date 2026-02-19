using KafkaConsumer.Kafka.Values;

namespace KafkaConsumer.Kafka
{
    public class ConsumerOnBackground : BackgroundService
    {
        private readonly IConsumer kafkaConsumer;

        public ConsumerOnBackground(IConfiguration configuration, ISerializer serializer)
        {
            kafkaConsumer = new Consumer(configuration, serializer);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await kafkaConsumer.ConsumeMessagesFromKafkaAsync(stoppingToken);
        }
    }
}
