namespace REST.Models
{
    public class Message
    {
        public required int Id { get; set; }
        public required int RoomId { get; set; }
        public required int SenderId { get; set; }
        public required string Content { get; set; }
        public required DateTime Time { get; set; }
    }
}
