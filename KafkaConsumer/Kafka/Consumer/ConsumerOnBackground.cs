using KafkaConsumer.Data;

namespace KafkaConsumer.Kafka
{
    public class ConsumerOnBackground : BackgroundService
    {
        private readonly IConsumer kafkaConsumer;

        public ConsumerOnBackground(IConfiguration configuration, IDataAccess dataAccess)
        {
            kafkaConsumer = new Consumer(configuration, dataAccess);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await kafkaConsumer.ConsumeMessagesFromKafkaAsync(stoppingToken);
        }
    }
}
