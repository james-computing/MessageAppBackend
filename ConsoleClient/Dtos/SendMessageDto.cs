using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Dtos
{
    public class SendMessageDto
    {
        public required int RoomId { get; set; }
        public required string Content { get; set; }
        public DateTime Time { get; set; }
    }
}
