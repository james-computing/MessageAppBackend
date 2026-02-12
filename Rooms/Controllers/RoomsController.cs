using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rooms.Data;
using Rooms.Roles;

namespace Rooms.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize]
    public class RoomsController(IDataAccess dataAccess) : ControllerBase
    {
    }
}
