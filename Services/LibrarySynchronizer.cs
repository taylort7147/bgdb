using System.Diagnostics;
using BggExt.Data;
using BggExt.Models;
using Microsoft.EntityFrameworkCore;

namespace BggExt.Services;

public class LibrarySynchronizer(
    IServiceScopeFactory _scopeFactory,
    XmlApi2.Api _api,
    ImageStore _imageStore,
    ILogger<LibrarySynchronizer> _logger)
{
    public enum Status { Success, Error, Pending };

    // This may not be an efficient way of updating the DB, but I don't need it
    // to be, and probably won't be able to justify the time to make it
    // efficient.
    public async Task<Status> Synchronize(CancellationToken stoppingToken, string libraryId)
    {
        _logger.LogInformation($"Synchronizing library '{libraryId}'");
        var logTime = (string name, Stopwatch timer) =>
        {
            _logger.LogInformation($"[{nameof(Synchronize)} Timer][Library '{libraryId}'][{name}'] {timer.Elapsed}.");
        };

        // Verify that the library exists
        if (!await LibraryExists(libraryId))
        {
            _logger.LogError($"Unable to find library '{libraryId}'");
            return Status.Error;
        }

        // Start a timer for the full synchronization process
        var totalSyncTimer = Stopwatch.StartNew();

        var apiTimer = Stopwatch.StartNew();
        var result = await _api.GetCollection(libraryId);
        logTime("XML API call", apiTimer);
        apiTimer.Stop();

        if (result.Status == XmlApi2.ApiResult.OperationStatus.Success)
        {
            var boardGames = result.Data as IList<XmlApi2.BoardGame>;
            if (boardGames == null)
            {
                _logger.LogError($"Could not cast result data from {nameof(XmlApi2.Api.GetCollection)} to {nameof(IList<XmlApi2.BoardGame>)}");
                return Status.Error;
            }

            var allBoardGamesSyncTimer = Stopwatch.StartNew();
            var boardGameEntities = new List<BoardGame>();
            foreach (var boardGame in boardGames)
            {
                var boardGameSyncTimer = Stopwatch.StartNew();
                var boardGameEntity = await AddOrUpdateBoardGame(boardGame);
                boardGameEntities.Add(boardGameEntity);
                var mechanicEntities = new List<Mechanic>();
                var categoryEntities = new List<Category>();
                var familyEntities = new List<Family>();
                foreach (var mechanic in boardGame.Mechanics!)
                {
                    var mechanicEntity = await AddMechanic(mechanic);
                    mechanicEntities.Add(mechanicEntity);
                }
                foreach (var category in boardGame.Categories!)
                {
                    var categoryEntity = await AddCategory(category);
                    categoryEntities.Add(categoryEntity);
                }
                foreach (var family in boardGame.Families!)
                {
                    var familyEntity = await AddFamily(family);
                    familyEntities.Add(familyEntity);
                }
                await UpdateBoardGameMechanics(boardGame.Id, mechanicEntities);
                await UpdateBoardGameCategories(boardGame.Id, categoryEntities);
                await UpdateBoardGameFamilies(boardGame.Id, familyEntities);

                logTime("Single board game", boardGameSyncTimer);
            }
            await UpdateLibraryBoardGames(libraryId, boardGameEntities);
            logTime($"All {boardGames.Count} board games", allBoardGamesSyncTimer);
            var averageTime = allBoardGamesSyncTimer.Elapsed / boardGames.Count;
            _logger.LogInformation($"[{nameof(Synchronize)} Timer][Library '{libraryId}'][Average board game sync] {averageTime}.");
            logTime("Total synchronization process", totalSyncTimer);
            return Status.Success;
        }
        else if (result.Status == XmlApi2.ApiResult.OperationStatus.Pending)
        {
            _logger.LogWarning($"XML API returned pending status: {result.Message}");
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

    private async Task<bool> LibraryExists(string libraryId)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<BoardGameDbContext>();

            // Verify that the library exists
            return await context.Libraries
                .Where(l => l.Id == libraryId)
                .AnyAsync();
        }
    }

    private async Task<BoardGame> AddOrUpdateBoardGame(XmlApi2.BoardGame boardGame)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<BoardGameDbContext>();
            var model = await context.BoardGames.Where(b => b.Id == boardGame.Id).FirstOrDefaultAsync()
                ?? new BoardGame();

            context.Entry(model).State = model.Id == 0 ? EntityState.Added : EntityState.Modified;

            model.Id = boardGame.Id;
            model.Name = boardGame.Name;
            model.YearPublished = boardGame.YearPublished;
            model.MinPlayers = boardGame.MinPlayers;
            model.MaxPlayers = boardGame.MaxPlayers;
            model.MinPlayTimeMinutes = boardGame.MinPlayTimeMinutes;
            model.MaxPlayTimeMinutes = boardGame.MaxPlayTimeMinutes;
            model.PlayingTimeMinutes = boardGame.PlayingTimeMinutes;
            model.AverageWeight = boardGame.AverageWeight;
            model.Description = boardGame.Description;
            model.MinAge = boardGame.MinAge;

            if (boardGame.Image != null)
            {
                model.ImageId = (await _imageStore.StoreImage(boardGame.Image)).Id;
            }

            if (boardGame.Thumbnail != null)
            {
                model.ThumbnailId = (await _imageStore.StoreImage(boardGame.Thumbnail)).Id;
            }

            await context.SaveChangesAsync();
            return model;
        }
    }

    private async Task<Mechanic> AddMechanic(XmlApi2.Link mechanic)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<BoardGameDbContext>();
            var model = await context.Mechanics.Where(m => m.Id == mechanic.Id).FirstOrDefaultAsync()
                ?? new Mechanic();
            if (model.Id == 0)
            {
                model.Id = mechanic.Id;
                model.Name = mechanic.Value;
                await context.Mechanics.AddAsync(model);
                await context.SaveChangesAsync();
            }
            return model;
        }
    }

    private async Task<Category> AddCategory(XmlApi2.Link category)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<BoardGameDbContext>();
            var model = await context.Categories.Where(c => c.Id == category.Id).FirstOrDefaultAsync()
                ?? new Category();
            if (model.Id == 0)
            {
                model.Id = category.Id;
                model.Name = category.Value;
                await context.Categories.AddAsync(model);
                await context.SaveChangesAsync();
            }
            return model;
        }
    }

    private async Task<Family> AddFamily(XmlApi2.Link family)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<BoardGameDbContext>();
            var model = await context.Families.Where(f => f.Id == family.Id).FirstOrDefaultAsync()
                ?? new Family();
            if (model.Id == 0)
            {
                model.Id = family.Id;
                model.Name = family.Value;
                await context.Families.AddAsync(model);
                await context.SaveChangesAsync();
            }
            return model;
        }
    }

    private async Task UpdateBoardGameMechanics(int boardGameId, IList<Mechanic> mechanics)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<BoardGameDbContext>();
            var boardGame = await context.BoardGames
                .Where(b => b.Id == boardGameId)
                .Include(b => b.Mechanics)
                .SingleAsync();

            foreach (var mechanic in mechanics)
            {
                if (!boardGame.Mechanics!.Where(m => m.Id == mechanic.Id).Any())
                {
                    boardGame.Mechanics!.Add(mechanic);
                    context.Entry(mechanic).State = EntityState.Unchanged;
                }
            }
            await context.SaveChangesAsync();
        }
    }

    private async Task UpdateBoardGameCategories(int boardGameId, IList<Category> categories)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<BoardGameDbContext>();
            var boardGame = await context.BoardGames
                .Where(b => b.Id == boardGameId)
                .Include(b => b.Categories)
                .SingleAsync();

            foreach (var category in categories)
            {
                if (!boardGame.Categories!.Where(m => m.Id == category.Id).Any())
                {
                    boardGame.Categories!.Add(category);
                    context.Entry(category).State = EntityState.Unchanged;
                }
            }
            await context.SaveChangesAsync();
        }
    }

    private async Task UpdateBoardGameFamilies(int boardGameId, IList<Family> families)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<BoardGameDbContext>();
            var boardGame = await context.BoardGames
                .Where(b => b.Id == boardGameId)
                .Include(b => b.Families)
                .SingleAsync();

            foreach (var family in families)
            {
                if (!boardGame.Families!.Where(m => m.Id == family.Id).Any())
                {
                    boardGame.Families!.Add(family);
                    context.Entry(family).State = EntityState.Unchanged;
                }
            }
            await context.SaveChangesAsync();
        }
    }

    private async Task UpdateLibraryBoardGames(string libraryId, IList<BoardGame> boardGames)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<BoardGameDbContext>();
            var library = await context.Libraries
                .Where(l => l.Id == libraryId)
                .Include(l => l.LibraryData)
                .SingleAsync();
            foreach (var libraryData in library.LibraryData)
            {
                if (!boardGames.Where(b => b.Id == libraryData.BoardGameId).Any())
                {
                    context.Entry(libraryData).State = EntityState.Deleted;
                }
                else
                {
                    context.Entry(libraryData).State = EntityState.Detached;
                }
            }
            foreach (var boardGame in boardGames)
            {
                if (!library.LibraryData.Where(d => d.BoardGameId == boardGame.Id).Any())
                {
                    context.LibraryData.Add(new BoardGameLibraryData
                    {
                        BoardGameId = boardGame.Id,
                        LibraryId = libraryId
                    });
                }
            }
            await context.SaveChangesAsync();
        }
    }
}
