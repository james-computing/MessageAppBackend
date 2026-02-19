namespace REST.Dtos.Messages
{
    public class EditMessageDto
    {
        public required int MessageId { get; set; }
        public required string NewContent { get; set; }
    }
}
