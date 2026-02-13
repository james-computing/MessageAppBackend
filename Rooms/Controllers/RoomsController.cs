using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rooms.Data;
using Rooms.Roles;
using System.Security.Claims;

namespace Rooms.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize]
    public class RoomsController(IDataAccess dataAccess) : ControllerBase
    {
        private async Task<int?> GetUserIdFromEmail(ClaimsPrincipal user)
        {
            string? userEmail = user.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
            {
                return null;
            }
            int userId = await dataAccess.GetUserIdFromEmail(userEmail);
            return userId;
        }
    }
}
