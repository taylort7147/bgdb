using System.ComponentModel.DataAnnotations;
using BggExt.Data;
using BggExt.Models;
using BggExt.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters.Xml;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;

namespace BggExt.Controllers;

[ApiController]
[Route("api/account")]
public class AccountController(
    BoardGameDbContext _context, XmlApi2.Api _api,
    ILogger<AccountController> _logger) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(ApplicationUserRegistration userRegistration,
        [FromServices] SignInManager<ApplicationUser> signInManager,
        [FromServices] UserManager<ApplicationUser> userManager)
    {
        if (await userManager.Users.Where(u => (u.Email != null) && u.Email.ToLower() == userRegistration.Email.ToLower()).CountAsync() != 0)
        {
            ModelState.AddModelError(nameof(userRegistration.Email), $"The email is already registered.");
        }
        if (await userManager.Users.Where(u => (u.UserName != null) && u.UserName.ToLower() == userRegistration.UserName.ToLower()).CountAsync() != 0)
        {
            ModelState.AddModelError(nameof(userRegistration.UserName), "The username is already registered.");
        }
        if (await _context.Libraries.Where(l => l.Id == userRegistration.BoardGameGeekUsername).CountAsync() != 0)
        {
            ModelState.AddModelError(nameof(userRegistration.BoardGameGeekUsername),
                $"The BoardGameGeek username is already registered.");
        }
        if (!ModelState.IsValid)
        {
            return ValidationProblem();
        }

        // Make call to BGG API and re-check model state
        var bggUsernameResult = await _api.GetUser(userRegistration.BoardGameGeekUsername);
        if (bggUsernameResult.Status == XmlApi2.ApiResult.OperationStatus.Error)
        {
            var errorListString = string.Join(", ", bggUsernameResult.Errors);
            ModelState.AddModelError(nameof(userRegistration.BoardGameGeekUsername),
            "A problem occurred while querying BoardGameGeek for username validation." +
                $" Errors: [{errorListString}]");
        }
        else if (bggUsernameResult.Status == XmlApi2.ApiResult.OperationStatus.Pending)
        {
            ModelState.AddModelError(nameof(userRegistration.BoardGameGeekUsername),
                "A problem occurred while querying BoardGameGeek for username validation." +
                $" Please try again later. Message: {bggUsernameResult.Message}");
        }
        if (!ModelState.IsValid)
        {
            return ValidationProblem();
        }

        var user = new ApplicationUser()
        {
            Email = userRegistration.Email,
            UserName = userRegistration.UserName,
            Library = new Library() { Id = userRegistration.BoardGameGeekUsername },
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
            return CreatedAtAction(nameof(Register), user);
        }
        _logger.LogError($"An error occurred while registering user '{userRegistration.UserName}' with email '{userRegistration.Email}'." +
            $" Errors: [{string.Join(", ", result.Errors.Select(e => e.Description))}]");
        return Problem(statusCode: StatusCodes.Status500InternalServerError);
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

    [Authorize]
    [HttpGet("library")]
    public async Task<IActionResult> GetLibrary(UserManager<ApplicationUser> userManager)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }
        return CreatedAtAction(nameof(GetLibrary), user.Library.LibraryData);
    }

    public class ApplicationUserDetails
    {
        public string Id { get; set; }

        public string? UserName { get; set; }

        public IList<string> Roles { get; set; }

        public ApplicationUserDetails(ApplicationUser user)
        {
            Id = user.Id;
            UserName = user.UserName;
            Roles = new List<string>();
        }
    }

    [Authorize]
    [HttpGet("details")]
    public async Task<IActionResult> GetDetails(
            [FromServices] UserManager<ApplicationUser> userManager,
            [FromServices] RoleManager<IdentityRole> roleManager)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var details = new ApplicationUserDetails(user);
        details.Roles = await userManager.GetRolesAsync(user);
        return CreatedAtAction(nameof(GetDetails), details);
    }
}
