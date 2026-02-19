namespace KafkaConsumer.Kafka
{
    public class ConsumerOnBackground : BackgroundService
    {
        private readonly IConsumer kafkaConsumer;

        public ConsumerOnBackground(IConfiguration configuration)
        {
            kafkaConsumer = new Consumer(configuration);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await kafkaConsumer.ConsumeMessagesFromKafkaAsync(stoppingToken);
        }
    }
}
