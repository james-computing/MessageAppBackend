using Client.Dtos;
using ConsoleClient.Clients.Auth;
using ConsoleClient.Clients.MessageRealTime;
using ConsoleClient.Clients.REST;
using ConsoleClient.Clients.Urls;
using ConsoleClient.Enums;
using Auth.Dtos;
using REST.Dtos.Messages;
using REST.Dtos.Rooms;
using REST.Roles;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ConsoleClient
{
    internal class Test
    {
        private Random random = new Random();

        private readonly int _usersQuantity;
        private readonly Url url;

        // Auth
        AuthClient authClient; // Only need a single Auth client to get tokens
        private UserRegisterDto[] userRegisterDtos;
        private TokenDto[] tokens;
        private int[] usersIds;
        private const string ALPHNUM = "ABCEDFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        private const string SYMB = @"&%-/\*#$";

        // REST
        RESTClient restClient; // Only need a single REST client
        // Rooms
        private const string originalRoomName = "original name";
        private const string newRoomName = "new name";

        // MessageRealTime
        private const uint numberOfMessagesToGenerate = 50;

        const string editedMessageContent = "edited message content";

        private const int DELAY_MILLISECS = 500;

        public Test(bool productionUrls, int usersQuantity)
        {
            if(usersQuantity < 4)
            {
                throw new Exception($"Test needs at least 4 users: usersQuantity = {usersQuantity}.");
            }
            _usersQuantity = usersQuantity;
            userRegisterDtos = new UserRegisterDto[usersQuantity];
            tokens = new TokenDto[usersQuantity];
            usersIds = new int[usersQuantity];

            url = new Url(productionUrls);
            
            authClient = new AuthClient(url);
            restClient = new RESTClient(url);
        }

        public async Task ExecuteAsync()
        {
            // Try to register new users until we have the amount of users we need
            await RegisterRandomUsersAsync();
            PrintRegisteredUsers();

            // Login each user
            await LoginUsersAsync();
            await TestRefreshingTokensAsync();
            GetUsersIdsFromTokens();

            // Create a room for the first user
            CreateRoomDto createRoomDto = new()
            {
                Name = originalRoomName 
            };
            int roomId = await restClient.CreateRoomAndAddUserToItAsync(tokens[0], createRoomDto);

            // Check that the user 0 only has a room, and that the id is correct
            IEnumerable<int> roomsIds = await restClient.GetUserRoomsIdsAsync(tokens[0]);
            if(roomsIds.Count() != 1)
            {
                throw new Exception("Error: user has incorrect number of rooms.");
            }
            if (roomsIds.First() != roomId)
            {
                throw new Exception("Error: got room with wrong id.");
            }

            for (int i = 1; i < _usersQuantity; i++)
            {
                await AddUserToRoomAsync(roomId, tokens[0], tokens[i]);
            }

            await CheckThatTheRoomCreatorIsTheOnlyAdminAsync(roomId, usersIds[0]);

            // Test if the user 1 can change the name of the room.
            // It shouldn't be possible.
            await TestRegularUserTryingToRenameARoomAsync(roomId, tokens[1]);
            // Turn the user 1 into an admin
            await TestPromotingRegularUserToAdminAsync(roomId, usersIds[1], tokens[0]);
            // Try again to rename the room, now with admin privilege
            await TestAdminRenamingARoomAsync(roomId, tokens[1]);
            // Remove the last user, which should be a regular user
            await TestAdminRemovingARegularUserAsync(roomId, tokens[0], usersIds[_usersQuantity-1]);
            // Also make an admin try to remove an admin, it shouldn't be possible.
            await TestAdminRemovingAdminAsync(roomId, tokens[0], usersIds[1]);
            // Also check that a regular user can't remove an admin, it shouldn't be possible.
            await TestRegularUserRemovingAdminAsync(roomId, tokens[_usersQuantity-2], usersIds[1]);

            // Do some chatting
            int numberOfUsersInRoom = _usersQuantity-1; // last user was removed from the room
            TokenDto[] tokensOfUsersInRoom = new TokenDto[numberOfUsersInRoom];
            for (int i = 0; i < numberOfUsersInRoom; i++)
            {
                tokensOfUsersInRoom[i] = tokens[i];
            }
            // Constructing the clients already starts the connections to SignalR
            MessageRealTimeClient[] mrtClients = await ConstructMessageRealTimeClients(tokensOfUsersInRoom);
            await ChatAsync(roomId, mrtClients);
            await StopConnectionsToSignalRAsync(mrtClients);

            Console.WriteLine("Finished chatting.");

            // Count the messages for user 0
            // Every user should have numberOfMessagesToGenerate messages
            int messageCount = mrtClients[0].CountMessages();
            if (mrtClients[0].CountMessages() != numberOfMessagesToGenerate)
            {
                throw new Exception($"Error: incorrect number of messages stored locally. Should be {numberOfMessagesToGenerate}, but have {messageCount}.");
            }

            Console.WriteLine("Messages of user 0:");
            mrtClients[0].PrintMessages();

            // Test editing, deleting and loading messages

            // Get all messages
            IOrderedEnumerable<ReceiveMessageDto> messages = mrtClients[0].GetAllMessagesInOrder();
            // Get two messages, one to delete, another to edit.
            IEnumerator<ReceiveMessageDto> enumerator = messages.GetEnumerator();
            enumerator.MoveNext();// To get the first, have to move next
            ReceiveMessageDto firstMessage = enumerator.Current;
            enumerator.MoveNext();
            ReceiveMessageDto secondMessage = enumerator.Current;
            enumerator.Dispose();

            int firstMessageSenderId = firstMessage.SenderId;
            int secondMessageSenderId = secondMessage.SenderId;

            // With the id, identify the index of the corresponding user
            int firstMessageSenderIndex = -1; // Initialize with an invalid index
            int secondMessageSenderIndex = -1; // Initialize with an invalid index
            int indicesObtained = 0;
            for (int i=0; i < numberOfUsersInRoom; i++)
            {
                if (usersIds[i] == firstMessageSenderId)
                {
                    firstMessageSenderIndex = i;
                    indicesObtained++;
                }

                if (usersIds[i] == secondMessageSenderId)
                {
                    secondMessageSenderIndex = i;
                    indicesObtained++;
                }

                if(indicesObtained == 2)
                {
                    break;
                }
            }

            if(indicesObtained != 2)
            {
                throw new Exception($"Error: couldn't get the indices of the senders. Got indices:\nfirstMessageSenderIndex = {firstMessageSenderIndex},\n secondMessageSenderIndex = {secondMessageSenderIndex}.");
            }

            // Delete the first message
            DeleteMessageDto deleteMessageDto = new()
            {
                MessageId = firstMessage.Id,
            };
            await restClient.DeleteMessageAsync(deleteMessageDto, tokens[firstMessageSenderIndex]);

            // Edit the second message
            EditMessageDto editMessageDto = new()
            {
                MessageId = secondMessage.Id,
                NewContent = editedMessageContent,
            };
            await restClient.EditMessageAsync(editMessageDto, tokens[secondMessageSenderIndex]);

            // Load last messages
            LoadLatestMessagesDto loadLatestMessagesDto = new()
            {
                RoomId = roomId,
                Quantity = numberOfMessagesToGenerate,
            };
            IEnumerable<REST.Models.Message> latestMessages =  await restClient.LoadLatestMessagesAsync(loadLatestMessagesDto, tokens[0]);

            // Check that the number of messages is correct
            int latestMessagesCount = latestMessages.Count();
            if (latestMessagesCount != numberOfMessagesToGenerate - 1)
            {
                throw new Exception($"Error: wrong number of latest messages. Got {latestMessagesCount}, should be {numberOfMessagesToGenerate-1}.");
            }

            // Check that the edited message have the correct content
            // Since the first message was deleted and the edited message was the second,
            // now the edited message is the first from the loaded lastest messages.
            IOrderedEnumerable<REST.Models.Message> latestMessagesOrdered = latestMessages.OrderBy(message => message.Time);
            REST.Models.Message editedMessage = latestMessagesOrdered.First();
            if(editedMessage.Id != secondMessage.Id)
            {
                throw new Exception("Error: didn't find the edited message in the expected position in the loaded latest messages.");
            }
            if(editedMessage.Content != editedMessageContent)
            {
                throw new Exception($"Error: edited message has wrong content. Got {editedMessage.Content}, should be {editedMessageContent}.");
            }

            // Load the messages again, using a message as reference.
            LoadMessagesFromReferenceDto loadMessagesPrecedingReferenceDto = new()
            {
                RoomId = roomId,
                MessageIdReference = latestMessagesOrdered.Last().Id,
                Quantity = (uint)latestMessagesCount,
            };
            IEnumerable<REST.Models.Message> loadedMessages = await restClient.LoadMessagesFromReferenceAsync(
                                                                loadMessagesPrecedingReferenceDto,
                                                                tokens[0]);

            // Check that we got the same number of messages
            int loadedMessagesCount = loadedMessages.Count();
            if(loadedMessagesCount !=  latestMessagesCount)
            {
                throw new Exception($"Error: loadedMessages with wrong number of messages. Got {loadedMessagesCount}, should be {latestMessagesCount}.");
            }

            // Check that we got the same messages again
            IEnumerator<REST.Models.Message> latestMessagesEnumerator = latestMessagesOrdered.GetEnumerator();
            latestMessagesEnumerator.MoveNext();
            foreach (REST.Models.Message message in loadedMessages.OrderBy(msg => msg.Time))
            {
                var current = latestMessagesEnumerator.Current;
                if (message.Id != current.Id)
                {
                    throw new Exception("Error: loaded messages differ from latest messages.");
                }
                latestMessagesEnumerator.MoveNext();
            }

            // Remove admins and check that the remaining users became admins
            // Remove the admins 0 and 1
            IEnumerable<TokenDto> tokensOfAdmins = [tokens[0], tokens[1]];
            await TestThatAllUsersBecomeAdminWhenAdminsLeaveAsync(roomId, tokensOfAdmins, tokens[2]);
            
            await CleanupAsync(roomId, tokens[2]);

            Console.WriteLine("Test succeeded.");
        }

        private async Task<bool> TryToRegisterARandomUserAsync(AuthClient authClient, int index)
        {
            string emailLeft = random.GetString(ALPHNUM, 10);
            string email = emailLeft + "@something.com";
            string username = random.GetString(ALPHNUM, 5);
            string password = random.GetString(ALPHNUM,15) + random.GetString(SYMB,15);

            UserRegisterDto randomDto = new()
            {
                Email = email,
                Username = username,
                Password = password,
            };

            bool succeded = await authClient.RegisterAsync(randomDto);

            if (succeded)
            {
                userRegisterDtos[index] = randomDto;
            }

            return succeded;
        }

        private async Task RegisterRandomUsersAsync()
        {
            // Keep trying to register new users
            Console.WriteLine("\nTrying to register new users...");
            int i = 0;
            bool registered;
            int count = 0;
            while (i < _usersQuantity)
            {
                Console.WriteLine($"Attempt {count}");
                registered = await TryToRegisterARandomUserAsync(authClient, i);
                if (registered)
                {
                    i++;
                }
                count++;
                await Task.Delay(DELAY_MILLISECS);
            }
            Console.WriteLine("Finished registering users.");
        }

        private void PrintRegisteredUsers()
        {
            Console.WriteLine("Users registered:");
            for (int i = 0; i < _usersQuantity; i++)
            {
                Console.WriteLine($"\n{i}");
                Console.WriteLine($"Email = {userRegisterDtos[i].Email}");
                Console.WriteLine($"Username = {userRegisterDtos[i].Username}");
                Console.WriteLine($"Password = {userRegisterDtos[i].Password}");
            }
        }

        private async Task LoginUsersAsync()
        {
            Console.WriteLine("\nLogin users...");
            for (int i = 0; i < _usersQuantity; i++)
            {
                TokenDto? token = null;
                while (token == null)
                {
                    UserLoginDto userLoginDto = new()
                    {
                        Email = userRegisterDtos[i].Email,
                        Password = userRegisterDtos[i].Password,
                    };
                    token = await authClient.LoginAsync(userLoginDto);
                    if(token != null)
                    {
                        // Clone the token
                        tokens[i] = new TokenDto()
                        {
                            AccessToken = token.AccessToken,
                            RefreshToken = token.RefreshToken,
                        };
                    }
                    await Task.Delay(DELAY_MILLISECS);
                }
            }
            Console.WriteLine("Finished login users.");
        }

        private int GetIdFromJWT(string token)
        {
            JwtSecurityTokenHandler handler = new();

            JwtSecurityToken jwtToken = handler.ReadJwtToken(token);

            Claim claim = jwtToken.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier);
            string idString = claim.Value;
            int id = Int32.Parse(idString);

            return id;
        }

        private void GetUsersIdsFromTokens()
        {
            for (int i = 0; i < _usersQuantity; i++)
            {
                usersIds[i] = GetIdFromJWT(tokens[i].AccessToken);
            }
        }

        private async Task DeleteUsersAsync()
        {
            Console.WriteLine("\nDeleting users...");
            for (int i = 0; i < _usersQuantity; i++)
            {
                bool succeded = false;
                while (!succeded)
                {
                    succeded = await authClient.DeleteAsync(tokens[i]);
                    await Task.Delay(DELAY_MILLISECS);
                }
            }
            Console.WriteLine("Finished deleting users.");
        }

        private async Task TestRefreshingTokensAsync()
        {
            for (int i=0; i < _usersQuantity; i++)
            {
                TokenDto? refreshedToken = await authClient.RefreshAccessTokenAsync(tokens[i]);
                if(refreshedToken == null)
                {
                    throw new Exception("Error: failed to refresh token.");
                }
                tokens[i] = refreshedToken;
                await Task.Delay(DELAY_MILLISECS);
            }
        }

        public async Task AddUserToRoomAsync(int roomId, TokenDto roomCreatorToken, TokenDto userToJoinToken)
        {
            // Get a room invitation
            GenerateInvitationTokenDto generateInvitationTokenDto = new()
            {
                RoomId = roomId
            };
            string invitationToken = await restClient.GenerateInvitationTokenAsync(roomCreatorToken, generateInvitationTokenDto);
            Console.WriteLine($"Invitation token = {invitationToken}");

            // Join the room
            JoinRoomDto joinRoomDto = new()
            {
                InvitationToken = invitationToken,
            };
            await restClient.JoinRoomAsync(userToJoinToken, joinRoomDto);
        }

        public async Task CheckThatTheRoomCreatorIsTheOnlyAdminAsync(int roomId, int roomCreatorId)
        {
            GetUsersInfoFromRoomDto getUsersInfoFromRoomDto = new()
            { 
                RoomId = roomId
            };
            IEnumerable <UserInfoDto> usersInfo = await restClient.GetUsersInfoFromRoomAsync(tokens[1], getUsersInfoFromRoomDto);

            foreach (UserInfoDto info in usersInfo)
            {
                if (info.Id == roomCreatorId && info.RoleInRoom != RoleInRoom.Admin.ToString())
                {
                    throw new Exception("Error: Room creator not admin.");
                }
                else if (info.Id != roomCreatorId && info.RoleInRoom == RoleInRoom.Admin.ToString())
                {
                    throw new Exception("Error: Room admin that should be regular.");
                }
            }
        }

        public async Task TestRegularUserTryingToRenameARoomAsync(int roomId, TokenDto regularUserToken)
        {
            UpdateRoomNameDto updateRoomNameDto = new()
            {
                Name = newRoomName,
                RoomId = roomId,
            };

            // We need finer control, so use RequestWithJsonAsync directly
            HttpResponseMessage responseMessage = await restClient.RequestWithJsonAsync(
                regularUserToken,
                HttpMethod.Put,
                MessageAppService.REST,
                MessageAppController.Rooms,
                RoomsAction.UpdateRoomName.ToString(),
                updateRoomNameDto);
            if (responseMessage.StatusCode != System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("Error: A regular user should receive a Forbidden response when trying to update the name of a room.");
            }
        }

        public async Task TestPromotingRegularUserToAdminAsync(int roomId, int regularUserId, TokenDto adminToken)
        {
            UpdateUserRoleInRoomDto updateUserRoleInRoomDto = new()
            {
                RoleInRoom = RoleInRoom.Admin,
                UserId = regularUserId,
                RoomId = roomId,
            };
            await restClient.UpdateUserRoleInRoomAsync(adminToken, updateUserRoleInRoomDto);

            // Get users info from room, to verify their roles
            GetUsersInfoFromRoomDto getUsersInfoFromRoomDto = new()
            {
                RoomId = roomId,
            };
            IEnumerable <UserInfoDto> usersInfo = await restClient.GetUsersInfoFromRoomAsync(adminToken, getUsersInfoFromRoomDto);

            // Check that user 1 is admin
            foreach (UserInfoDto info in usersInfo)
            {
                if (info.Id == usersIds[1] && info.RoleInRoom != RoleInRoom.Admin.ToString())
                {
                    throw new Exception("Error: Regular user not turned into admin.");
                }
            }
        }

        public async Task TestAdminRenamingARoomAsync(int roomId, TokenDto adminToken)
        {
            UpdateRoomNameDto updateRoomNameDto = new()
            {
                Name = newRoomName,
                RoomId = roomId
            };
            await restClient.UpdateRoomNameAsync(adminToken, updateRoomNameDto);
            
            // Get the room name
            GetRoomInfoDto getRoomInfoDto = new()
            {
                RoomId = updateRoomNameDto.RoomId,
            };
            RoomInfoDto roomInfo = await restClient.GetRoomInfoAsync(adminToken, getRoomInfoDto);

            // Check that the room name was updated
            if (roomInfo.Name != newRoomName)
            {
                throw new Exception("Error: Room name not updated.");
            }
        }

        public async Task TestAdminRemovingARegularUserAsync(int roomId, TokenDto adminToken, int regularUserId)
        {
            RemoveUserFromRoomDto removeUserFromRoomDto = new()
            {
                RoomId = roomId,
                UserId = regularUserId,
            };
            await restClient.RemoveUserFromRoomAsync(adminToken, removeUserFromRoomDto);

            GetUsersInfoFromRoomDto getUsersInfoFromRoomDto = new()
            { 
                RoomId = removeUserFromRoomDto.RoomId
            };

            // Get the users info again to verify that the user was removed
            IEnumerable<UserInfoDto> usersInfo = await restClient.GetUsersInfoFromRoomAsync(adminToken, getUsersInfoFromRoomDto);

            foreach (UserInfoDto info in usersInfo)
            {
                if (info.Id == regularUserId)
                {
                    throw new Exception("Error: Regular user not removed from room.");
                }
            }
        }

        public async Task TestAdminRemovingAdminAsync(int roomId, TokenDto adminToken, int adminToRemoveId)
        {
            RemoveUserFromRoomDto removeUserFromRoomDto = new()
            {
                RoomId = roomId,
                UserId = adminToRemoveId,
            };
            // We need finer control, so use the RequestWithJsonAsync method
            HttpResponseMessage responseMessage = await restClient.RequestWithJsonAsync(
                adminToken,
                HttpMethod.Delete,
                MessageAppService.REST,
                MessageAppController.Rooms,
                RoomsAction.RemoveUserFromRoom.ToString(),
                removeUserFromRoomDto);
            if (responseMessage.StatusCode != System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("Error: When trying to remove an admin, a Forbidden response should be received.");
            }

            GetUsersInfoFromRoomDto getUsersInfoFromRoomDto = new()
            {
                RoomId = removeUserFromRoomDto.RoomId
            };

            // Get the users info again to verify that the admin wasn't removed
            IEnumerable<UserInfoDto> usersInfo = await restClient.GetUsersInfoFromRoomAsync(adminToken, getUsersInfoFromRoomDto);

            // Check if the admin is still in the room
            bool containsTheAdmin = false;
            foreach (UserInfoDto info in usersInfo)
            {
                if (info.Id == adminToRemoveId)
                {
                    containsTheAdmin = true;
                }
            }

            if (!containsTheAdmin)
            {
                throw new Exception("Error: Admin was removed from room.");
            }
        }

        private async Task TestRegularUserRemovingAdminAsync(int roomId, TokenDto regularUserToken, int adminId)
        {
            RemoveUserFromRoomDto removeUserFromRoomDto = new()
            {
                RoomId = roomId,
                UserId = adminId,
            };
            // We need finer control, so use the RequestWithJsonAsync method
            HttpResponseMessage responseMessage = await restClient.RequestWithJsonAsync(
                regularUserToken,
                HttpMethod.Delete,
                MessageAppService.REST,
                MessageAppController.Rooms,
                RoomsAction.RemoveUserFromRoom.ToString(),
                removeUserFromRoomDto);
            if (responseMessage.StatusCode != System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("Error: When trying to remove an admin, a Forbidden response should be received.");
            }

            // Check if the admin is still in the room
            bool containsTheAdmin = false;
            // Get the users info again to verify that the admin wasn't removed
            GetUsersInfoFromRoomDto getUsersInfoFromRoomDto = new()
            {
                RoomId = removeUserFromRoomDto.RoomId
            };
            IEnumerable <UserInfoDto> usersInfo = await restClient.GetUsersInfoFromRoomAsync(
                                                    regularUserToken,
                                                    getUsersInfoFromRoomDto);
            foreach (UserInfoDto info in usersInfo)
            {
                if (info.Id == adminId)
                {
                    containsTheAdmin = true;
                }
            }

            if (!containsTheAdmin)
            {
                throw new Exception("Error: Admin was removed from room.");
            }
        }

        private async Task<MessageRealTimeClient[]> ConstructMessageRealTimeClients(TokenDto[] tokensOfUsersInRoom)
        {
            MessageRealTimeClient[] mrtClients = new MessageRealTimeClient[tokensOfUsersInRoom.Length];
            int numberOfUsersInRoom = mrtClients.Length;
            // Connect each user to SignalR, so they can chat
            Task[] tasks = new Task[numberOfUsersInRoom];
            for (int i = 0; i < numberOfUsersInRoom; i++)
            {
                mrtClients[i] = new MessageRealTimeClient(url, tokensOfUsersInRoom[i]);
                tasks[i] = mrtClients[i].TryToConnectToChatHubAsync();
            }
            Task.WaitAll(tasks);
            return mrtClients;
        }

        private async Task ChatAsync(int roomId, MessageRealTimeClient[] mrtClients)
        {
            int numberOfUsersInRoom = mrtClients.Length;

            Console.WriteLine("Chatting...");
            // Generate messages randomly
            for (int i = 0; i < numberOfMessagesToGenerate; i++)
            {
                int randomIndex = random.Next(numberOfUsersInRoom);
                string randomContent = random.GetString(ALPHNUM, 100);
                await mrtClients[randomIndex].SendMessageAsync(roomId, randomContent);
            }
        }

        private async Task StopConnectionsToSignalRAsync(MessageRealTimeClient[] mrtClients)
        {
            int numberOfUsersInRoom = mrtClients.Length;
            // Close the SignalR connections
            Task[] tasks = new Task[numberOfUsersInRoom];
            for (int i = 0; i < numberOfUsersInRoom; i++)
            {
                tasks[i] = mrtClients[i].StopAsync();
            }
            Task.WaitAll(tasks);
        }

        private async Task LeaveRoomAsync(int roomId, TokenDto token)
        {
            
            RemoveUserFromRoomDto removeUserFromRoomDto = new()
            {
                RoomId = roomId,
                UserId = GetIdFromJWT(token.AccessToken),
            };
            await restClient.RemoveUserFromRoomAsync(token, removeUserFromRoomDto);
        }

        private async Task TestThatAllUsersBecomeAdminWhenAdminsLeaveAsync(
            int roomId,
            IEnumerable<TokenDto> adminsTokens,
            TokenDto nonAdmintoken)
        {
            // Remove all admins
            int numberOfAdmins = adminsTokens.Count();
            List<Task> tasks = new(numberOfAdmins);
            foreach (TokenDto token in adminsTokens)
            {
                tasks.Add(LeaveRoomAsync(roomId, token));
            }
            Task.WaitAll(tasks);

            // Test that all remaining users became admins
            GetUsersInfoFromRoomDto getUsersInfoFromRoomDto = new()
            {
                RoomId = roomId,
            };
            IEnumerable<UserInfoDto> usersInfo = await restClient.GetUsersInfoFromRoomAsync(nonAdmintoken, getUsersInfoFromRoomDto);
            foreach (UserInfoDto userInfo in usersInfo)
            {
                if(userInfo.RoleInRoom != RoleInRoom.Admin.ToString())
                {
                    throw new Exception("Error: All users should be admins after all admins left the room.");
                }
            }
        }

        private async Task CleanupAsync(int roomId, TokenDto adminToken)
        {
            // Delete the room
            DeleteRoomDto deleteRoomDto = new()
            {
                RoomId = roomId
            };
            await restClient.DeleteRoomAsync(adminToken, deleteRoomDto);

            // Delete users
            await DeleteUsersAsync();
        }
    }
}
