using BggExt.Data;
using BggExt.Web;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using BggExt.Models;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Net.Mime;

namespace BggExt.Services;

public class LibrarySynchronizer(
    IServiceScopeFactory _scopeFactory,
    XmlApi2.Api _api,
    ILogger<LibrarySynchronizer> _logger)
{
    public enum Status { Success, Error, Pending };

    public async Task<Status> Synchronize(CancellationToken stoppingToken, string libraryId)
    {
        var result = await _api.GetCollection(libraryId);
        if (result.Status == XmlApi2.ApiResult.OperationStatus.Success)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<BoardGameDbContext>();
                var library = await context.Libraries.Where(l => l.Id == libraryId)
                    .Include(l => l.LibraryData)
                    .ThenInclude(d => d.BoardGame)
                    .FirstAsync();
                if(library == null)
                {
                    _logger.LogError($"Unable to find library '{libraryId}'");
                    return Status.Error;
                }
                context.Libraries.Attach(library);
                var boardGames = result.Data as IList<XmlApi2.BoardGame>;
                if (boardGames == null)
                {
                    _logger.LogError($"Could not cast result data from {nameof(XmlApi2.Api.GetCollection)} to {nameof(IList<XmlApi2.BoardGame>)}");
                    return Status.Error;
                }

                // Remove any games from the library that are no longer there
                for (var i = library.LibraryData.Count - 1; i != 0; --i)
                {
                    var libraryItem = library.LibraryData[i];
                    if (boardGames.Where(b => b.Id == libraryItem.BoardGame.Id).Count() == 0)
                    {
                        _logger.LogInformation($"Removing board game '{libraryItem.BoardGame.Name}' from library '{library.Id}'");
                        library.LibraryData.RemoveAt(i);
                    }
                }

                foreach (var boardGame in boardGames)
                {
                    // Add/update the board game in the database
                    var model = await context.BoardGames.Where(g => g.Id == boardGame.Id).FirstOrDefaultAsync();
                    var isNew = model == null;
                    if (model == null)
                    {
                        model = new BoardGame();
                    }

                    model.Name = boardGame.Name;
                    model.Id = boardGame.Id;
                    model.YearPublished = boardGame.YearPublished;
                    model.MinPlayers = boardGame.MinPlayers;
                    model.MaxPlayers = boardGame.MaxPlayers;
                    model.MinPlayTimeMinutes = boardGame.MinPlayTimeMinutes;
                    model.MaxPlayTimeMinutes = boardGame.MaxPlayTimeMinutes;
                    model.PlayingTimeMinutes = boardGame.PlayingTimeMinutes;
                    model.AverageWeight = boardGame.AverageWeight;
                    model.Description = boardGame.Description;
                    model.MinAge = boardGame.MinAge;
                    // TODO: The rest of the fields.

                    if (isNew)
                    {
                        _logger.LogInformation($"Game added: [{model.Id}] {model.Name}");
                        context.BoardGames.Add(model);
                    }
                    else
                    {
                        _logger.LogInformation($"Game updated: [{model.Id}] {model.Name}");
                        context.BoardGames.Update(model);
                    }

                    // Add the game to the library if it doesn't exist
                    var libraryData = await context.LibraryData
                        .Where(d =>
                            d.Library.Id.ToLower() == library.Id.ToLower() &&
                            d.BoardGame.Id == boardGame.Id)
                        .FirstOrDefaultAsync();
                    if (libraryData == null)
                    {
                        _logger.LogInformation($"Adding board game '{model.Name}' to library '{library.Owner}'");
                        await context.LibraryData.AddAsync(
                            new BoardGameLibraryData { BoardGame = model, Library = library });
                    }
                }

                await context.SaveChangesAsync();
                return Status.Success;
            }
        }
        else if (result.Status == XmlApi2.ApiResult.OperationStatus.Pending)
        {
            return Status.Pending;
        }
        else
        {
            _logger.LogError($"Errors occurred during synchronization of library '{libraryId}'");
            foreach (var error in result.Errors)
            {
                _logger.LogError(error);
            }
            return Status.Error;
        }

    }
}
