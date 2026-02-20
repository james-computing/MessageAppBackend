using ConsoleClient.Clients.Auth;
using JWTAuth.Dtos;

namespace ConsoleClient
{
    internal class Test
    {
        private readonly bool _productionUrls;
        private readonly int _usersQuantity = 3;
        public Test(bool productionUrls, int usersQuantity)
        {
            _productionUrls = productionUrls;
            _usersQuantity = usersQuantity;
        public async Task ExecuteAsync()
        {
            // Only need a single Auth client to get tokens
            AuthClient authClient = new AuthClient(_productionUrls);

        }
        }
    }
}
