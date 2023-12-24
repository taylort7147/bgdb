using BggExt.Data;
using BggExt.Models;
using BggExt.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BggExt.Controllers;

[ApiController]
[Route("library")]
public class LibraryController(BoardGameDbContext _context) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetLibrary(string id)
    {
        var library = await _context.Libraries.Where(l => l.Id.ToLower() == id.ToLower()).FirstOrDefaultAsync();
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
}
