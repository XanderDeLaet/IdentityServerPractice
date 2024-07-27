using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

public class CustomProfileService : IProfileService
{
    private readonly UserManager<IdentityUser> _userManager;

    public CustomProfileService(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        // Get the user based on the subject (i.e., the user ID) from the context
        var user = await _userManager.GetUserAsync(context.Subject);

        if (user != null)
        {
            // Get roles for the user
            var roles = await _userManager.GetRolesAsync(user);

            // Convert roles to claims
            var claims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();

            // Add claims to the context's issued claims
            context.IssuedClaims.AddRange(claims);
        }
    }

    public Task IsActiveAsync(IsActiveContext context)
    {
        // Set the user as active
        context.IsActive = true;
        return Task.CompletedTask;
    }
}
