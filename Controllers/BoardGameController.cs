using BggExt.Data;
using BggExt.Models;
using BggExt.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BggExt.Controllers;

[ApiController]
[Route("api/boardgame")]
public class BoardGameController(BoardGameDbContext _context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetGames(CancellationToken token)
    {
        var boardGames = _context.BoardGames
            .Include(boardGame => boardGame.Mechanics)
            .Include(boardGame => boardGame.Categories)
            .Include(boardGame => boardGame.Families)
            .Include(boardGame => boardGame.Thumbnail)
            .Include(boardGame => boardGame.Image)
            .AsAsyncEnumerable();

        return new CreatedAtActionResult(nameof(GetGames), "BoardGames", null, await boardGames.Select(MapBoardGame).ToListAsync());

        BoardGame MapBoardGame(BoardGame boardGame) => new()
        {
            Id = boardGame.Id,
            Name = boardGame.Name,
            Description = boardGame.Description,
            YearPublished = boardGame.YearPublished,
            MinPlayers = boardGame.MinPlayers,
            MaxPlayers = boardGame.MaxPlayers,
            PlayingTimeMinutes = boardGame.PlayingTimeMinutes,
            MinPlayTimeMinutes = boardGame.MinPlayTimeMinutes,
            MaxPlayTimeMinutes = boardGame.MaxPlayTimeMinutes,
            MinAge = boardGame.MinAge,
            AverageWeight = boardGame.AverageWeight,
            Mechanics = boardGame.Mechanics,
            Categories = boardGame.Categories,
            Families = boardGame.Families,
            Thumbnail = boardGame.Thumbnail,
            Image = boardGame.Image
        };
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetGameAsync(int id)
    {
        var game = await _context.BoardGames
            .Include(b => b.Mechanics)
            .Include(b => b.Categories)
            .Include(b => b.Families)
            .Where(b => b.Id == id)
            .FirstOrDefaultAsync();
        
        if(game == null)
        {
            return NotFound();
        }

        return new OkObjectResult(game);
    }

    [HttpGet("mechanics")]
    [AllowAnonymous]
    public async Task<IList<Mechanic>> GetMechanicsAsync()
    {
        var mechanics = await _context.Mechanics.OrderBy(m => m.Name).ToListAsync();
        return mechanics;
    }

    [HttpGet("categories")]
    [AllowAnonymous]
    public async Task<IList<Category>> GetCategoriesAsync()
    {
        var categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
        return categories;
    }

    [HttpGet("families")]
    [AllowAnonymous]
    public async Task<IList<Family>> GetFamiliesAsync()
    {
        var families = await _context.Families.OrderBy(f => f.Name).ToListAsync();
        return families;
    }
}
