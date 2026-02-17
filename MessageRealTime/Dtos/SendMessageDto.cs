namespace MessageRealTime.Dtos
{
    public class SendMessageDto
    {
        public required int RoomId { get; set; }
        public required string Content { get; set; }
        public DateTime Time { get; set; }
    }
}
