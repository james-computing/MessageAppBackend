namespace REST.Dtos.Messages
{
    public class LoadMessagesFromReferenceDto
    {
        public required int RoomId { get; set; }
        public required int MessageIdReference { get; set; }
        public required uint Quantity { get; set; }
    }
}
