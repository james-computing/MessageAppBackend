using Auth.Dtos;
using Auth.Models;
using System.Globalization;

namespace Auth.Services
{
    public interface IAuthService
    {
        public Task<User?> RegisterAsync(UserRegisterDto userRegisterDto);
        public Task<TokenDto?> LoginAsync(UserLoginDto userLoginDto);
        public Task<TokenDto?> RefreshAccessTokenAsync(int userId, string refreshToken);
        public Task DeleteAsync(int userId);
    }
}
