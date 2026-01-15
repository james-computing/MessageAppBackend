using JWTAuth.Dtos;
using JWTAuth.Models;

namespace JWTAuth.Data
{
    public interface IDataAccess
    {
        public Task RegisterUser(User user);
        public Task<bool> UserExists(UserRegisterDto userDto);
        public Task<User?> GetUserFromId(int userId);
        public Task<User?> GetUserFromEmail(string userEmail);
        public Task SaveRefreshToken(int userId, RefreshTokenData refreshTokenData);
        public Task<RefreshTokenData?> GetRefreshTokenData(int userId);
    }
}
