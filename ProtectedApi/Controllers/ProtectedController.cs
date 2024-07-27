using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProtectedApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProtectedController : ControllerBase
    {
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Get()
        {
            return Ok(new { message = "This is a protected route for Admins" });
        }

        [HttpGet("user")]
        public IActionResult GetUser()
        {
            return Ok(new { message = "This is a protected route for Users" });
        }
    }
}
