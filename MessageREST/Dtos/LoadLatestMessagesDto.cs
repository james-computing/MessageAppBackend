namespace MessageREST.Dtos
{
    public class LoadLatestMessagesDto
    {
        public required int RoomId { get; set; }
        public required uint Quantity { get; set; }
    }
}
