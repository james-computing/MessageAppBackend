namespace REST.Dtos.Rooms
{
    public class RemoveUserFromRoomDto
    {
        public required int RoomId { get; set; }
        public required string UserEmail { get; set; }
    }
}
