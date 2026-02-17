namespace KafkaConsumer.Data
{
    public interface IDataAccess
    {
        public Task SaveMessage(int senderId, int roomId, string content, DateTime time);
    }
}
