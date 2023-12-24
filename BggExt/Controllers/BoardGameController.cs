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
[Route("[controller]")]
public class BoardGameController(BoardGameDbContext _context, XmlApi2.Api _api) : ControllerBase
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
        var result = await _api.GetBoardGame(id);
        if (result.Status != XmlApi2.ApiResult.OperationStatus.Success)
        {
            return NotFound();
        }

        var b = result.Data as XmlApi2.BoardGame ?? throw new InvalidCastException("Could not cast XmlApi2 result data to BoardGame.");
        BoardGame boardGame = new()
        {
            Id = b.Id,
            Name = b.Name,
            Description = b.Description,
            YearPublished = b.YearPublished,
            MinPlayers = b.MinPlayers,
            MaxPlayers = b.MaxPlayers,
            PlayingTimeMinutes = b.PlayingTimeMinutes,
            MinPlayTimeMinutes = b.MinPlayTimeMinutes,
            MaxPlayTimeMinutes = b.MaxPlayTimeMinutes,
            MinAge = b.MinAge,
            AverageWeight = b.AverageWeight,
            Mechanics = b.Mechanics.Select(m => new Mechanic() { Name = m.Value, Id = m.Id }).ToList(),
            Categories = b.Categories.Select(m => new Category() { Name = m.Value, Id = m.Id }).ToList(),
            Families = b.Families.Select(m => new Family() { Name = m.Value, Id = m.Id }).ToList(),
            Thumbnail = null,
            Image = null
        };
        return new CreatedAtActionResult(nameof(BoardGameController), nameof(GetGame), new { id = id }, boardGame);
    }
}
