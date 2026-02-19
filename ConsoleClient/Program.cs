using JWTAuth.Dtos;

namespace Client
{
    internal class Program
    {
        public static async Task Main()
        {
            bool productionUrls = false;

            UserRegisterDto userRegisterDto = new UserRegisterDto()
            {
                Email = "john@hotmail.com",
                Password = "john123",
                Username = "John",
            };
        }
    }
}