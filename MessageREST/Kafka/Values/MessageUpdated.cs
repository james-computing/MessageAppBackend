using System.Text.Json;

namespace MessageREST.Kafka.Values
{
    public class MessageUpdated
    {
        public required int MessageId { get; set; }
        public required int RoomId { get; set; }
    }
}
