namespace MessageREST.Dtos
{
    public class EditMessageDto
    {
        public required int MessageId { get; set; }
        public required string NewContent { get; set; }
    }
}
