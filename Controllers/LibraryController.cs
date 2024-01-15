using BggExt.Data;
using BggExt.Models;
using BggExt.Models.Dto;
using BggExt.Services;
using BggExt.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

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
    public async Task<IActionResult> GetLibraryAsync(string id, bool includeGames = false, bool includeGameProperties = false)
    {
        var library = await GetLibraryInternalAsync(id, includeGames, includeGameProperties);
        if (library == null)
        {
            return NotFound($"Library '{id}' was not found");
        }
        var dto = new LibraryDto();
        dto.Id = library.Id;
        dto.LastSynchronized = library.LastSynchronized;
        dto.IsEnabled = library.IsEnabled;
        dto.OwnerId = library.Owner.Id;
        dto.LibraryData = library.LibraryData.Select(d => new BoardGameLibraryDataDto
        {
            Id = d.Id,
            LibraryId = d.LibraryId,
            Location = d.Location,
            BoardGame = new BoardGameDto
            {
                Id = d.BoardGameId,
                Name = d.BoardGame.Name,
                Description = d.BoardGame.Description,
                MinPlayers = d.BoardGame.MinPlayers,
                MaxPlayers = d.BoardGame.MaxPlayers,
                MinPlayTimeMinutes = d.BoardGame.MinPlayTimeMinutes,
                MaxPlayTimeMinutes = d.BoardGame.MaxPlayTimeMinutes,
                PlayingTimeMinutes = d.BoardGame.PlayingTimeMinutes,
                MinAge = d.BoardGame.MinAge,
                AverageWeight = d.BoardGame.AverageWeight,
                YearPublished = d.BoardGame.YearPublished,
                ImageId = d.BoardGame.ImageId,
                ThumbnailId = d.BoardGame.ThumbnailId,
                Mechanics = d.BoardGame.Mechanics!.Select(m => m.Name).ToList(),
                Categories = d.BoardGame.Categories!.Select(c => c.Name).ToList(),
                Families = d.BoardGame.Families!.Select(f => f.Name).ToList()
            }
        }).ToList();
        return CreatedAtAction(nameof(GetLibraryAsync), dto);
    }

    public class LibraryDataEdit
    {
        public string? Location { get; set; }
    }

    [Authorize]
    [HttpPost("data/edit/{id}")]
    public async Task<IActionResult> EditGame(int id,
        [FromBody] LibraryDataEdit editData,
        [FromServices] UserManager<ApplicationUser> userManager)
    {
        var libraryData = await _context.LibraryData
            .Where(d => d.Id == id)
            .Include(d => d.Library)
            .ThenInclude(l => l.Owner)
            .FirstOrDefaultAsync();
        if (libraryData == null)
        {
            return NotFound();
        }

        if (!await DoesUserHaveEditPermission(userManager, libraryData.Library))
        {
            return Unauthorized("Only the owner of this library or an admin may use this API.");
        }

        _context.LibraryData.Attach(libraryData);
        libraryData.Location = editData.Location;
        await _context.SaveChangesAsync();
        return Ok();
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

    private async Task<Library?> GetLibraryInternalAsync(string id, 
        bool includeGames = false, 
        bool includeGameProperties = false)
    {
        var library = _context.Libraries
            .Include(l => l.Owner)
            .Where(l => l.Id.ToLower() == id.ToLower());
        if (includeGames)
        {
            if (includeGameProperties)
            {
                library = library
                    .Include(l => l.LibraryData)
                        .ThenInclude(d => d.BoardGame)
                            .ThenInclude(g => g.Mechanics)
                    .Include(l => l.LibraryData)
                        .ThenInclude(d => d.BoardGame)
                            .ThenInclude(g => g.Categories)
                    .Include(l => l.LibraryData)
                        .ThenInclude(d => d.BoardGame)
                            .ThenInclude(g => g.Families);
            }
            else
            {
                library = library
                    .Include(l => l.LibraryData)
                        .ThenInclude(d => d.BoardGame);
            }
        }
        return await library.FirstOrDefaultAsync();
    }

    [AllowAnonymous]
    [HttpGet("canedit")]
    public async Task<IActionResult> CanEditAsync(string id, [FromServices] UserManager<ApplicationUser> userManager)
    {
        var library = await GetLibraryInternalAsync(id);
        if (library == null)
        {
            return BadRequest($"Library '{id}' was not found");
        }
        var result = await DoesUserHaveEditPermission(userManager, library);
        return CreatedAtAction(nameof(CanEditAsync), result);
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost("setsyncstate/{id}")]
    public async Task<IActionResult> SetSyncState(
        string id,
        [FromBody] bool isEnabled,
        [FromServices] ISynchronizationJobQueue jobQueue)
    {
        var library = await GetLibraryInternalAsync(id);
        if (library == null)
        {
            return BadRequest($"Library '{id}' was not found");
        }
        library.IsEnabled = isEnabled;

        if (isEnabled)
        {
            await jobQueue.QueueJobAsync(id);
        }

        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(SetSyncState), library.IsEnabled);
    }

    [Authorize(Roles = "Administrator,LibraryOwner")]
    [HttpPost("sync/{id}")]
    public async Task<IActionResult> Synchronize(string id,
        [FromServices] UserManager<ApplicationUser> userManager,
        [FromServices] ISynchronizationJobQueue jobQueue)
    {
        var library = await GetLibraryInternalAsync(id);
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
