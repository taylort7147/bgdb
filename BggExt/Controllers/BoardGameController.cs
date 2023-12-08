using BggExt.Data;
using BggExt.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BggExt.Controllers;

[ApiController]
[Route("[controller]")]
public class BoardGameController(BoardGameDbContext _context) : ControllerBase
{
    [HttpGet]
    public IAsyncEnumerable<BoardGame> GetGames(CancellationToken token)
    {
        var boardGames = _context.BoardGames
            .Include(boardGame => boardGame.Mechanics)
            .Include(boardGame => boardGame.Categories)
            .Include(boardGame => boardGame.Families)
            .Include(boardGame => boardGame.Thumbnail)
            .Include(boardGame => boardGame.Image)
            .AsAsyncEnumerable();

        return boardGames.Select(MapBoardGame);

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
}