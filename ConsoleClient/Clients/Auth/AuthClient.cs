using ConsoleClient.Clients.Urls;
using ConsoleClient.Enums;
using JWTAuth.Dtos;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ConsoleClient.Clients.Auth
{
    internal class AuthClient
    {
        // Serialization options
        private readonly JsonSerializerOptions jsonSerializerOptions;

        // Urls to communicate with server
        private readonly Url _url;

        public AuthClient(Url url)
        {
            Console.WriteLine("Constructing Auth client...");

            jsonSerializerOptions = new JsonSerializerOptions();
            jsonSerializerOptions.PropertyNameCaseInsensitive = true;

            _url = url;

            Console.WriteLine("Finished constructing Auth client.");
        }

        public async Task<bool> TestConnectionToAuthAsync()
        {
            Console.WriteLine("Trying to connect to Auth service...");
            HttpClient httpClient = new HttpClient();

            // Test connection first
            string url = _url.FromControllerAction(
                Service.Auth,
                Controller.Auth,
                AuthAction.TestConnection.ToString());
            HttpResponseMessage responseMessageConnectionTest = await httpClient.GetAsync(url);
            if (!responseMessageConnectionTest.IsSuccessStatusCode)
            {
                Console.WriteLine("Error: Failed to connect to Auth service.");
                return false;
            }
            else
            {
                Console.WriteLine("Successfully connected to Auth service.");
                string responseConnectionTest = await responseMessageConnectionTest.Content.ReadAsStringAsync();
                Console.WriteLine($"{responseConnectionTest}");
                return true;
            }
        }

        public async Task<bool> RegisterAsync(UserRegisterDto userRegisterDto)
        {
            Console.WriteLine("Trying to register new user...");

            HttpClient httpClient = new HttpClient();

            string serializedJson = JsonSerializer.Serialize(userRegisterDto);
            Console.WriteLine($"json to post {serializedJson}");
            using StringContent jsonContent = new StringContent(serializedJson, Encoding.UTF8, "application/json");

            string url = _url.FromControllerAction(
                Service.Auth,
                Controller.Auth,
                AuthAction.Register.ToString());
            HttpResponseMessage responseMessage = await httpClient.PostAsync(url, jsonContent);
            if (responseMessage.IsSuccessStatusCode)
            {
                string response = await responseMessage.Content.ReadAsStringAsync();
                Console.WriteLine($"response: {response}");
                return true;
            }

            Console.WriteLine("Error: Failed to register new user or error in server");
            return false;
        }

        public async Task<TokenDto?> LoginAsync(UserLoginDto userLoginDto)
        {
            Console.WriteLine("Trying to log in...");

            HttpClient httpClient = new HttpClient();

            string serializedString = JsonSerializer.Serialize(userLoginDto);
            Console.WriteLine($"json to post:\n{serializedString}\n");

            using StringContent jsonContent = new(serializedString, Encoding.UTF8, "application/json");

            string url = _url.FromControllerAction(
                Service.Auth,
                Controller.Auth,
                AuthAction.Login.ToString());
            HttpResponseMessage responseMessage = await httpClient.PostAsync(url, jsonContent);
            if (responseMessage.IsSuccessStatusCode)
            {
                Console.WriteLine("Login successful.");
                string content = await responseMessage.Content.ReadAsStringAsync();
                Console.WriteLine(content);
                return JsonSerializer.Deserialize<TokenDto>(content, jsonSerializerOptions);
            }

            Console.WriteLine($"Error: Failed to log in. Status code: {responseMessage.StatusCode}");
            return null;
        }

        public async Task<bool> DeleteUserAsync(TokenDto token)
        {
            Console.WriteLine("Trying to delete user...");
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

            string url = _url.FromControllerAction(
                Service.Auth,
                Controller.Auth,
                AuthAction.Delete.ToString());
            HttpResponseMessage response = await httpClient.DeleteAsync(url);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Delete a user.");
                return true;
            }
            else
            {
                Console.WriteLine($"Error: Failed to delete. Status code = {response.StatusCode}");
                return false;
            }
        }
    }   
}
