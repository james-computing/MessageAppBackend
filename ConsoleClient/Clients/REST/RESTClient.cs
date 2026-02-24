using ConsoleClient.Clients.Urls;
using ConsoleClient.Enums;
using JWTAuth.Dtos;
using REST.Dtos.Messages;
using REST.Dtos.Rooms;
using REST.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace ConsoleClient.Clients.REST
{
    internal class RESTClient
    {
        // Urls to communicate with server
        private readonly Url _url;

        private readonly JsonSerializerOptions jsonSerializerOptions;

        public RESTClient(Url url)
        {
            Console.WriteLine("Constructing REST client...");
            _url = url;
            jsonSerializerOptions = new JsonSerializerOptions();
            jsonSerializerOptions.PropertyNameCaseInsensitive = true;
            Console.WriteLine("Finished constructing REST client.");
        }

        public async Task<HttpResponseMessage> RequestWithJsonAsync(
            TokenDto token,
            HttpMethod method,
            MessageAppService service,
            MessageAppController controller,
            string action,
            object dto)
        {
            HttpClient httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

            string serializedJson = JsonSerializer.Serialize(dto);
            Console.WriteLine($"json to post {serializedJson}");
            using StringContent jsonContent = new StringContent(serializedJson, Encoding.UTF8, "application/json");

            string url = _url.FromControllerAction(
                service,
                controller,
                action);

            HttpRequestMessage request = new();
            request.Method = method;
            request.Content = jsonContent;
            request.RequestUri = new Uri(url);

            HttpResponseMessage responseMessage = await httpClient.SendAsync(request);
            return responseMessage;
        }

        public async Task<int> CreateRoomAndAddUserToItAsync(TokenDto token, CreateRoomDto createRoomDto)
        {
            Console.WriteLine("Trying to create a new room...");

            HttpResponseMessage responseMessage = await RequestWithJsonAsync(
                token,
                HttpMethod.Post,
                MessageAppService.REST,
                MessageAppController.Rooms,
                RoomsAction.CreateRoomAndAddUserToIt.ToString(),
                createRoomDto);

            if (responseMessage.IsSuccessStatusCode)
            {
                int roomId = await responseMessage.Content.ReadFromJsonAsync<int>();
                Console.WriteLine($"Created room with roomId = {roomId}");
                return roomId;
            }
            string content = await responseMessage.Content.ReadAsStringAsync();
            throw new Exception($"Error: Failed to create room. Status code: {responseMessage.StatusCode}.");
        }

        public async Task DeleteRoomAsync(TokenDto token, DeleteRoomDto deleteRoomDto)
        {
            Console.WriteLine("Trying to delete room...");

            HttpResponseMessage responseMessage = await RequestWithJsonAsync(
                token,
                HttpMethod.Delete,
                MessageAppService.REST,
                MessageAppController.Rooms,
                RoomsAction.DeleteRoom.ToString(),
                deleteRoomDto);

            if (responseMessage.IsSuccessStatusCode)
            {
                Console.WriteLine("Room deleted successfully.");
                return;
            }
            else
            {
                throw new Exception($"Error: Failed to delete room. Status code: {responseMessage.StatusCode}.");
            }
        }

        public async Task UpdateRoomNameAsync(TokenDto token, UpdateRoomNameDto updateRoomNameDto)
        {
            Console.WriteLine("Trying to rename room...");
            
            HttpResponseMessage responseMessage = await RequestWithJsonAsync(
                token,
                HttpMethod.Put,
                MessageAppService.REST,
                MessageAppController.Rooms,
                RoomsAction.UpdateRoomName.ToString(),
                updateRoomNameDto);

            if (responseMessage.IsSuccessStatusCode)
            {
                Console.WriteLine("Room renamed successfully.");
                return;
            }
            else
            {
                throw new Exception($"Error: Failed to rename room. Status code: {responseMessage.StatusCode}.");
            }
        }

        public async Task<string> GenerateInvitationTokenAsync(TokenDto token, GenerateInvitationTokenDto generateInvitationTokenDto)
        {
            Console.WriteLine("Trying to generate invitation...");

            HttpResponseMessage responseMessage = await RequestWithJsonAsync(
                token,
                HttpMethod.Post,
                MessageAppService.REST,
                MessageAppController.Rooms,
                RoomsAction.GenerateInvitationToken.ToString(),
                generateInvitationTokenDto);

            if (responseMessage.IsSuccessStatusCode)
            {
                string content = await responseMessage.Content.ReadAsStringAsync();
                return content;
            }
            else
            {
                throw new Exception($"Error: Failed to generate invitation. Status code: {responseMessage.StatusCode}.");
            }
        }

        public async Task JoinRoomAsync(TokenDto token, JoinRoomDto joinRoomDto)
        {
            Console.WriteLine("Trying to join room...");

            HttpResponseMessage responseMessage = await RequestWithJsonAsync(
                token,
                HttpMethod.Post,
                MessageAppService.REST,
                MessageAppController.Rooms,
                RoomsAction.JoinRoom.ToString(),
                joinRoomDto);

            if (responseMessage.IsSuccessStatusCode)
            {
                return;
            }
            else
            {
                throw new Exception($"Error: Failed to join room. Status code: {responseMessage.StatusCode}.");
            }
        }

        public async Task RemoveUserFromRoomAsync(TokenDto token, RemoveUserFromRoomDto removeUserFromRoomDto)
        {
            Console.WriteLine("Trying to remove user from room...");

            HttpResponseMessage responseMessage = await RequestWithJsonAsync(
                token,
                HttpMethod.Delete,
                MessageAppService.REST,
                MessageAppController.Rooms,
                RoomsAction.RemoveUserFromRoom.ToString(),
                removeUserFromRoomDto);

            if (responseMessage.IsSuccessStatusCode)
            {
                return;
            }
            else
            {
                throw new Exception($"Error: Failed to remove user from room. Status code: {responseMessage.StatusCode}.");
            }
        }

        public async Task UpdateUserRoleInRoomAsync(TokenDto token, UpdateUserRoleInRoomDto updateUserRoleInRoomDto)
        {
            Console.WriteLine("Trying to update user role in room...");

            HttpResponseMessage responseMessage = await RequestWithJsonAsync(
                token,
                HttpMethod.Put,
                MessageAppService.REST,
                MessageAppController.Rooms,
                RoomsAction.UpdateUserRoleInRoom.ToString(),
                updateUserRoleInRoomDto);

            if (responseMessage.IsSuccessStatusCode)
            {
                return;
            }
            else
            {
                throw new Exception($"Error: Failed to update user role in room. Status code: {responseMessage.StatusCode}.");
            }
        }

        public async Task<RoomInfoDto> GetRoomInfoAsync(TokenDto token, GetRoomInfoDto getRoomInfoDto)
        {
            Console.WriteLine("Trying to get room info...");

            HttpResponseMessage responseMessage = await RequestWithJsonAsync(
                token,
                HttpMethod.Get,
                MessageAppService.REST,
                MessageAppController.Rooms,
                RoomsAction.GetRoomInfo.ToString(),
                getRoomInfoDto);

            if (responseMessage.IsSuccessStatusCode)
            {
                string content = await responseMessage.Content.ReadAsStringAsync();
                RoomInfoDto roomInfo = JsonSerializer.Deserialize<RoomInfoDto>(content, jsonSerializerOptions)
                                        ?? throw new Exception("Error: Failed to deserialize content to RoomInfoDto.");
                return roomInfo;
            }
            else
            {
                throw new Exception($"Error: Failed to get room info. Status code: {responseMessage.StatusCode}.");
            }
        }

        public async Task<IEnumerable<UserInfoDto>> GetUsersInfoFromRoomAsync(
            TokenDto token,
            GetUsersInfoFromRoomDto getUsersInfoFromRoomDto)
        {
            Console.WriteLine("Trying to get users info from room...");

            HttpResponseMessage responseMessage = await RequestWithJsonAsync(
                token,
                HttpMethod.Get,
                MessageAppService.REST,
                MessageAppController.Rooms,
                RoomsAction.GetUsersInfoFromRoom.ToString(),
                getUsersInfoFromRoomDto);

            if (responseMessage.IsSuccessStatusCode)
            {
                string content = await responseMessage.Content.ReadAsStringAsync();
                Console.WriteLine(content);
                IEnumerable<UserInfoDto> usersInfo = JsonSerializer.Deserialize<IEnumerable<UserInfoDto>>(content,jsonSerializerOptions)
                    ?? throw new Exception("Error: Failed to deserialize content to IEnumerable<UserInfoDto>.");
                return usersInfo;
            }
            else
            {
                throw new Exception($"Error: Failed to get users info from room. Status code: {responseMessage.StatusCode}.");
            }
        }

        //************************************* message service ******************************************
        public async Task<IEnumerable<Message>> LoadLatestMessagesAsync(LoadLatestMessagesDto loadLatestMessagesDto, TokenDto token)
        {
            Console.WriteLine("Trying to load last messages from room...");

            HttpResponseMessage responseMessage = await RequestWithJsonAsync(
                token,
                HttpMethod.Get,
                MessageAppService.REST,
                MessageAppController.Message,
                MessageAction.LoadLatestMessages.ToString(),
                loadLatestMessagesDto);

            if (responseMessage.IsSuccessStatusCode)
            {
                string content = await responseMessage.Content.ReadAsStringAsync();
                IEnumerable<Message> messages = JsonSerializer.Deserialize<IEnumerable<Message>>(content, jsonSerializerOptions)
                    ?? throw new Exception("Error: Failed to deserialize content to IEnumerable<Message>.");
                return messages;
            }
            else
            {
                throw new Exception($"Error: Failed to load last messages from room. Status code: {responseMessage.StatusCode}.");
            }
        }

        public async Task<IEnumerable<Message>> LoadMessagesFromReferenceAsync(
            LoadMessagesPrecedingReferenceDto loadMessagesPrecedingRefDto,
            TokenDto token)
        {
            Console.WriteLine("Trying to load messages from room...");

            HttpResponseMessage responseMessage = await RequestWithJsonAsync(
                token,
                HttpMethod.Get,
                MessageAppService.REST,
                MessageAppController.Message,
                MessageAction.LoadMessagesPrecedingReference.ToString(),
                loadMessagesPrecedingRefDto);

            if (responseMessage.IsSuccessStatusCode)
            {
                string content = await responseMessage.Content.ReadAsStringAsync();
                IEnumerable<Message> messages = JsonSerializer.Deserialize<IEnumerable<Message>>(content, jsonSerializerOptions)
                    ?? throw new Exception("Error: Failed to deserialize content to IEnumerable<Message>.");
                return messages;
            }
            else
            {
                throw new Exception($"Error: Failed to load messages from room. Status code: {responseMessage.StatusCode}.");
            }
        }

        public async Task EditMessageAsync(EditMessageDto editMessageDto, TokenDto token)
        {
            Console.WriteLine("Trying to edit message...");

            HttpResponseMessage responseMessage = await RequestWithJsonAsync(
                token,
                HttpMethod.Put,
                MessageAppService.REST,
                MessageAppController.Message,
                MessageAction.EditMessage.ToString(),
                editMessageDto);

            if (responseMessage.IsSuccessStatusCode)
            {
                return;
            }
            else
            {
                throw new Exception($"Error: Failed to edit message. Status code: {responseMessage.StatusCode}.");
            }
        }

        public async Task DeleteMessageAsync(DeleteMessageDto deleteMessageDto, TokenDto token)
        {
            Console.WriteLine("Trying to delete message...");

            HttpResponseMessage responseMessage = await RequestWithJsonAsync(
                token,
                HttpMethod.Delete,
                MessageAppService.REST,
                MessageAppController.Message,
                MessageAction.DeleteMessage.ToString(),
                deleteMessageDto);

            if (responseMessage.IsSuccessStatusCode)
            {
                return;
            }
            else
            {
                throw new Exception($"Error: Failed to delete message. Status code: {responseMessage.StatusCode}.");
            }
        }
    }
}
