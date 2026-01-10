using JWTAuth.Dtos;
using JWTAuth.Models;
using JWTAuth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JWTAuth.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        // Create a user in the database
        [HttpPost]
        public async Task<ActionResult> RegisterAsync(UserDto userDto)
        {
            User? user = await authService.RegisterAsync(userDto);
            if(user == null)
            {
                return BadRequest("User already exists.");
            }

            return Created();
        }

    }
}
