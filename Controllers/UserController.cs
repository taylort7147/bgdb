using BggExt.Data;
using BggExt.Models;
using BggExt.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;

namespace BggExt.Controllers;

[ApiController]
[Route("api/user")]
public class UserController(UserManager<ApplicationUser> _userManager) : ControllerBase
{
    [HttpGet("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> GetUser(string id)
    {
        var user = await _userManager.Users.Where(u => u.Id == id).Include(u => u.Library).FirstOrDefaultAsync();
        if (user == null)
        {
            return BadRequest($"User '{id}' could not be found.");
        }
        return CreatedAtAction(nameof(GetUser), user);
    }

    [HttpGet]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> GetUsers()
    {
        return CreatedAtAction(nameof(GetUsers), await _userManager.Users.Include(u => u.Library).ToListAsync());
    }

    [Authorize(Roles = "Administrator")]
    [HttpGet("roles")]
    public async Task<IActionResult> GetRoles(
        [FromQuery] string? userId,
        [FromServices] RoleManager<IdentityRole> roleManager,
        [FromServices] UserManager<ApplicationUser> userManager)
    {
        if (userId == null)
        {
            return CreatedAtAction(nameof(GetRoles), await roleManager.Roles.Select(r => r.Name).ToListAsync());
        }
        else
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest($"User '{userId}' could not be found.");
            }
            var roles = await userManager.GetRolesAsync(user);
            return CreatedAtAction(nameof(GetRoles), roles);
        }
    }

    public class UserAndRole
    {
        public string UserId { get; set; } = default!;
        public string Role { get; set; } = default!;
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost("addrole")]
    public async Task<IActionResult> AddRole(
        [FromBody] UserAndRole userAndRole,
        [FromServices] BoardGameDbContext context,
        [FromServices] UserManager<ApplicationUser> userManager)
    {
        var userId = userAndRole.UserId;
        var role = userAndRole.Role;

        var user = await userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return BadRequest($"User '{userId}' could not be found.");
        }

        var applicationRole = await context.Roles
            .Where(r => r.Name!.ToLower() == role.ToLower())
            .Select(r => r.Name)
            .FirstOrDefaultAsync();

        if (applicationRole == null)
        {
            return BadRequest($"Role '{role}' does not exist.");
        }

        if (!await userManager.IsInRoleAsync(user, applicationRole))
        {
            var result = await userManager.AddToRoleAsync(user, applicationRole);
            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Failed to add user '{userId}' to role '{applicationRole}'.");
            }
        }
        return Ok();
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost("removerole")]
    public async Task<IActionResult> RemoveRole(
        [FromBody] UserAndRole userAndRole,
        [FromServices] BoardGameDbContext context,
        [FromServices] UserManager<ApplicationUser> userManager)
    {
        var userId = userAndRole.UserId;
        var role = userAndRole.Role;

        var user = await userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return BadRequest($"User '{userId}' could not be found.");
        }

        var applicationRole = await context.Roles
            .Where(r => r.Name!.ToLower() == role.ToLower())
            .Select(r => r.Name)
            .FirstOrDefaultAsync();

        if (applicationRole == null)
        {
            return BadRequest($"Role '{role}' does not exist.");
        }

        if (await userManager.IsInRoleAsync(user, applicationRole))
        {
            var result = await userManager.RemoveFromRoleAsync(user, applicationRole);
            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Failed to remove user '{userId}' from role '{applicationRole}'.");
            }
        }
        return Ok();
    }

    
    [Authorize(Roles = "Administrator")]
    [HttpPost("delete")]
    public async Task<IActionResult> DeleteUser(
        [FromBody] string userId,
        [FromServices] BoardGameDbContext context,
        [FromServices] UserManager<ApplicationUser> userManager)
    {        
        var user = await context.Users
            .Include(u => u.Library)
            .ThenInclude(l => l.LibraryData)
            .Where(u => u.Id.ToLower() == userId.ToLower())
            .FirstOrDefaultAsync();
        if (user == null)
        {
            return BadRequest($"User '{userId}' could not be found.");
        }

        context.LibraryData.RemoveRange(user.Library.LibraryData);
        context.Libraries.Remove(user.Library);
        context.Users.Remove(user);
        await context.SaveChangesAsync();

        return Ok();
    }
}
