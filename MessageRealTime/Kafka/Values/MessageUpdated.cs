using System.Text.Json;

namespace MessageRealTime.Kafka.Values
{
    public class MessageUpdated
    {
        public required int MessageId { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
