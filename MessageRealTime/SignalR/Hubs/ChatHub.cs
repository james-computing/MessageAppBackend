using MessageRealTime.Data;
using MessageRealTime.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;


/*
    The Hub is transient, it can be disposed fast. For this reason, it can't be used for long tasks.
    The Hub.Context is null in the constructor, as far as my tests show, only use it in the methods.
*/

namespace MessageRealTime.SignalR.Hubs
{
    [Authorize]
    public class ChatHub : Hub<IChatClient>
    {
        private readonly IDataAccess _dataAccess;

        public ChatHub(IConfiguration configuration, IDataAccess dataAccess)
        {
            Console.WriteLine("------------------------------------------------------");
            Console.WriteLine("Constructing ChatHub...");

            _dataAccess = dataAccess;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();

            NotificationDto notificationDto = new()
            {
                Content = "Connected.",
            };
            await Clients.Caller.ReceiveNotificationAsync(notificationDto);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {

            Console.WriteLine("User disconnected.");
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessageAsync(SendMessageDto sendMessageDto)
        {
            string? senderIdString = Context.UserIdentifier;
            if (senderIdString == null)
            {
                Console.WriteLine("Error: null Context.UserIdentifier.");
                return;
            }

            int senderId;
            bool parsed = Int32.TryParse(senderIdString, out senderId);
            if(!parsed)
            {
                Console.WriteLine("Error: Failed to convert Context.UserIdentifier to int.");
                return;
            }

            // Get all users from the room
            IEnumerable<int> usersIds = await _dataAccess.GetUsersIdsFromRoom(sendMessageDto.RoomId);

            bool senderIsInRoom = false;
            // Alloc space for a list of the same size of usersIds.
            // If the sender is in the room, we will not be using one entry of the list,
            // but that is not much.
            // If the sender isn't in the room, then we need all space allocated.
            List<string> usersIdsToSend = new(usersIds.Count());
            // Copy the ids of the users in the room, except the sender id.
            // At the same time, check if the sender is in the room.
            // We'll use the list to broadcast the message to the users in the room,
            // except the sender.
            // SignalR needs the ids as strings, so also convert the ints to strings.
            foreach (int id in usersIds)
            {
                if(id == senderId)
                {
                    senderIsInRoom = true;
                }
                else
                {
                    usersIdsToSend.Add(id.ToString());
                }
            }

            // If the sender isn't in the room, it can't send a message to it, so return.
            if (!senderIsInRoom)
            {
                Console.WriteLine("User can't send a message to a room it is not in.");
                return;
            }

            // Save the message in the database, from which the message gets an id.
            // This id will be sent to the client as part of the message.
            Console.WriteLine("ChatHub saving message to database...");
            int messageId = await _dataAccess.SaveMessageAsync
                            (
                                sendMessageDto.RoomId,
                                senderId,
                                sendMessageDto.Content,
                                sendMessageDto.Time
                            );
            
            Console.WriteLine("ChatHub sending message to room...");
            ReceiveMessageDto receiveMessageDto = new()
            {
                Id = messageId,
                RoomId = sendMessageDto.RoomId,
                SenderId = senderId,
                Content = sendMessageDto.Content,
                Time = sendMessageDto.Time,
            };

            // Send message to all users in room except itself
            await Clients.Users(usersIdsToSend).ReceiveMessageAsync(receiveMessageDto);
        }
    }
}
