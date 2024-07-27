using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IdentityServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new IdentityUser { UserName = model.Username };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return Ok();
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("change-role")]
        public async Task<IActionResult> ChangeRole([FromBody] ChangeRoleModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Contains(model.NewRole))
            {
                return BadRequest("User already has the role");
            }

            // Remove current roles
            var resultRemove = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!resultRemove.Succeeded)
            {
                return BadRequest(resultRemove.Errors);
            }

            // Ensure the role exists
            if (!await _roleManager.RoleExistsAsync(model.NewRole))
            {
                await _roleManager.CreateAsync(new IdentityRole(model.NewRole));
            }

            // Add new role
            var resultAdd = await _userManager.AddToRoleAsync(user, model.NewRole);
            if (resultAdd.Succeeded)
            {
                return Ok();
            }

            return BadRequest(resultAdd.Errors);
        }
    }

    public class ChangeRoleModel
    {
        public string Username { get; set; }
        public string NewRole { get; set; }
    }
}
