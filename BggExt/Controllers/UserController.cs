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
[Route("user")]
public class UserController(UserManager<ApplicationUser> _userManager) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> GetUsers()
    {
        return CreatedAtAction(nameof(GetUsers), await _userManager.Users.Include(u => u.Library).ToListAsync());
    }
}
