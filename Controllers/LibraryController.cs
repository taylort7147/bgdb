using BggExt.Data;
using BggExt.Models;
using BggExt.Services;
using BggExt.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BggExt.Controllers;

[ApiController]
[Route("api/library")]
public class LibraryController(BoardGameDbContext _context) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetLibrariesAsync()
    {
        var libraries = await _context.Libraries.ToListAsync();
        return CreatedAtAction(nameof(GetLibrariesAsync), libraries);
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetLibraryAsync(string id, bool includeGames)
    {
        var library = await GetLibraryInternalAsync(id, includeGames);
        if (library == null)
        {
            return BadRequest($"Library '{id}' was not found");
        }
        return CreatedAtAction(nameof(GetLibraryAsync), library);
    }

    private async Task<bool> DoesUserHaveEditPermission(UserManager<ApplicationUser> userManager, Library library)
    {
        if (User.Identity == null || User.Identity.Name == null)
        {
            return false;
        }
        var user = await userManager.FindByNameAsync(User.Identity.Name)!;
        if (user == null)
        {
            return false;
        }
        var isAdmin = await userManager.IsInRoleAsync(user, "Administrator");
        var isOwner = user == library.Owner;
        return isOwner || isAdmin;
    }

    private async Task<Library?> GetLibraryInternalAsync(string id, bool includeGames)
    {
        var library = _context.Libraries.Where(l => l.Id.ToLower() == id.ToLower());
        if (includeGames)
        {
            library = library
            .Include(l => l.LibraryData)
            .ThenInclude(d => d.BoardGame);
        }
        return await library.FirstOrDefaultAsync();
    }

    [AllowAnonymous]
    [HttpGet("canedit")]
    public async Task<IActionResult> CanEditAsync(string id, [FromServices] UserManager<ApplicationUser> userManager)
    {
        var library = await GetLibraryInternalAsync(id, includeGames: false);
        if (library == null)
        {
            return BadRequest($"Library '{id}' was not found");
        }
        var result = await DoesUserHaveEditPermission(userManager, library);
        return CreatedAtAction(nameof(CanEditAsync), result);
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost("setsyncstate/{id}")]
    public async Task<IActionResult> SetSyncState(string id, [FromBody] bool isEnabled)
    {
        var library = await GetLibraryInternalAsync(id, false);
        if (library == null)
        {
            return BadRequest($"Library '{id}' was not found");
        }
        library.IsSynchronizationEnabled = isEnabled;

        // TODO: Add/remove library to/from sync service

        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(SetSyncState), library.IsSynchronizationEnabled);
    }

    [Authorize(Roles = "Administrator,LibraryOwner")]
    [HttpPost("sync/{id}")]
    public async Task<IActionResult> Synchronize(string id,
        [FromServices] UserManager<ApplicationUser> userManager,
        [FromServices] ISynchronizationJobQueue jobQueue)
    {
        var library = await GetLibraryInternalAsync(id, includeGames: false);
        if (library == null)
        {
            return BadRequest($"Library '{id}' does not exist.");
        }

        if (User.Identity == null || User.Identity.Name == null)
        {
            // Should never happen due to authorization requirement
            return Unauthorized();
        }
        if (!await DoesUserHaveEditPermission(userManager, library))
        {
            return Unauthorized("Only the owner of this library or an admin may use this API.");
        }

        await jobQueue.QueueJobAsync(id);
        return Ok();
    }
}
