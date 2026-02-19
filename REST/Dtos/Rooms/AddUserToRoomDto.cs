using REST.Roles;

namespace REST.Dtos.Rooms
{
    public class AddUserToRoomDto
    {
        public required int RoomId { get; set; }
        public required string UserEmail { get; set; }
        public required RoleInRoom RoleInRoom { get; set; }
    }
}
