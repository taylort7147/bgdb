using BggExt.Data;
using BggExt.Models;
using BggExt.Web;
using Microsoft.AspNetCore.Authorization;
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
    public async Task<IActionResult> GetGame(int id)
    {
        var game = await _context.BoardGames
            .Include(b => b.Mechanics)
            .Include(b => b.Categories)
            .Include(b => b.Families)
            .Include(b => b.Image)
            .Include(b => b.Thumbnail)
            .Where(b => b.Id == id)
            .FirstOrDefaultAsync();
        
        if(game == null)
        {
            return NotFound();
        }

        return CreatedAtAction(nameof(GetGame), game);
    }
}
