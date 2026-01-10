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

        // Login with credentials. It returns a JWT that should be used by the client to identity itself.
        [HttpPost]
        public async Task<ActionResult<TokenDto?>> LoginAsync(UserDto userDto)
        {
            TokenDto? token = await authService.LoginAsync(userDto);
            if(token == null)
            {
                return BadRequest();
            }
            return Ok(token);
        }
    }
}
