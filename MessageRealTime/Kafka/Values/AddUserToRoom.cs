using System.Text.Json;

namespace MessageRealTime.Kafka.Values
{
    public class AddUserToRoom
    {
        public required int RoomId { get; set; }
        public required int UserId { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
