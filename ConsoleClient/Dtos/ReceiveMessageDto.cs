using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Dtos
{
    public class ReceiveMessageDto
    {
        public required int Id { get; set; }
        public required int RoomId { get; set; }
        public required int SenderId { get; set; }
        public required string Content { get; set; }
        public DateTime Time { get; set; }
    }
}
