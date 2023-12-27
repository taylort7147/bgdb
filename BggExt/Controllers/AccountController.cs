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
[Route("api/account")]
public class AccountController(BoardGameDbContext _context) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(ApplicationUserRegistration userRegistration,
        [FromServices] SignInManager<ApplicationUser> signInManager,
        [FromServices] UserManager<ApplicationUser> userManager)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        if (await userManager.Users.Where(u => u.Email.ToLower() == userRegistration.Email.ToLower()).CountAsync() != 0)
        {
            return BadRequest($"The email '{userRegistration.Email}' is already registered.");
        }
        if (await _context.Libraries.Where(l => l.Id == userRegistration.LibraryId).CountAsync() != 0)
        {
            return BadRequest($"The library '{userRegistration.LibraryId}' is already registered.");
        }

        var user = new ApplicationUser()
        {
            Email = userRegistration.Email,
            UserName = userRegistration.Email,
            Library = new Library() { Id = userRegistration.LibraryId },
            NormalizedEmail = userRegistration.Email.ToLower()
        };

        var result = await userManager.CreateAsync(user, userRegistration.Password);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, "User");
            if (userManager.Users.Count() == 1)
            {
                // This is the first user - assume they are the administrator
                await userManager.AddToRoleAsync(user, "Administrator");
            }
            await _context.SaveChangesAsync();

            await signInManager.SignInAsync(user, isPersistent: false);
            return CreatedAtAction(nameof(Register), new { id = user.Id }, user);
        }
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
        return BadRequest();
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(SignInManager<ApplicationUser> signInManager,
        [FromBody] object empty)
    {
        if (empty != null)
        {
            await signInManager.SignOutAsync();
            return Ok();
        }
        return Unauthorized();
    }

    [HttpGet("library")]
    [Authorize]
    public async Task<IActionResult> GetLibrary(UserManager<ApplicationUser> userManager)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }
        return CreatedAtAction(nameof(GetLibrary), user.Library.LibraryData);
    }
}
