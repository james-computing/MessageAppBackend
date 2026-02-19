using System.Text.Json;

namespace REST.Kafka.Values
{
    public class RoomDeleted
    {
        public required int RoomId { get; set; }
    }
}
