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
[Route("library")]
public class LibraryController(BoardGameDbContext _context) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetLibrary(string id, bool includeGames)
    {
        var libraryQuery = _context.Libraries.Where(l => l.Id.ToLower() == id.ToLower());
        if(includeGames)
        {
            libraryQuery = libraryQuery
            .Include(l => l.LibraryData)
            .ThenInclude(d => d.BoardGame);
        }
        var library = await libraryQuery.FirstOrDefaultAsync();
        if (library == null)
        {
            return BadRequest($"Library '{id}' was not found");
        }
        return CreatedAtAction(nameof(GetLibrary), library);
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost("setsyncstate/{id}")]
    public async Task<IActionResult> SetSyncState(string id, [FromBody] bool isEnabled)
    {
        var library = await _context.Libraries.Where(l => l.Id.ToLower() == id.ToLower()).FirstAsync();
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
        var library = await _context.Libraries.Where(l => l.Id.ToLower() == id.ToLower()).FirstAsync();
        if (library == null)
        {
            return BadRequest($"Library '{id}' does not exist.");
        }

        if (User.Identity == null || User.Identity.Name == null)
        {
            // Should never happen due to authorization requirement
            return Unauthorized();
        }
        var user = await userManager.FindByNameAsync(User.Identity.Name)!;
        if (user == null)
        {
            // Should never happen due to authorization requirement
            return Unauthorized();
        }
        var isAdmin = await userManager.IsInRoleAsync(user, "Administrator");
        var isOwner = user == library.Owner;
        if (!isAdmin && !isOwner)
        {
            return Unauthorized("Only the owner of this library or an admin may use this API.");
        }

        await jobQueue.QueueJobAsync(id);
        return Ok();
    }
}
