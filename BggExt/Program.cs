using Asp.Versioning;
using BggExt;
using BggExt.Data;
using BggExt.Web;
using BggExt.XmlApi2;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using BoardGame = BggExt.Models.BoardGame;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews().AddJsonOptions(opts =>
    opts.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
);

builder.Services
    .AddConfiguredSwaggerUI()
    .AddSingleton<Downloader>()
    .AddSingleton<Api>()
    .AddHttpClient<Downloader>(c =>
    {
        // Set BaseURL and stuff here, more efficient than making a new client all the time
    })
    .SetHandlerLifetime(TimeSpan.FromMinutes(15))
    ;

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<BoardGameDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("BoardGameDbContext") ??
                          throw new InvalidOperationException("Connection string 'BoardGameDbContext' not found.")));
}
else
{
    builder.Services.AddDbContext<BoardGameDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("ProductionBoardGameDbContext")));
}


builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<BoardGameDbContext>();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
    app.UseHsts();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapIdentityApi<IdentityUser>();
app.UseConfiguredSwaggerUI();


app.MapControllerRoute(
    "default",
    "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");
app.MapPost("/logout", async (SignInManager<IdentityUser> signInManager,
    [FromBody] object empty) =>
    {
        if (empty != null)
        {
            await signInManager.SignOutAsync();
            return Results.Ok();
        }
        return Results.Unauthorized();
    })
    .WithOpenApi()
    .RequireAuthorization();

//Could also just add endpoints mapped to the app here rather than using controllers
var versionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1, 0))
    .HasApiVersion(new ApiVersion(2, 0))
    .ReportApiVersions()
    .Build();


// Ensure SQLite database is created
if (app.Environment.IsDevelopment())
{
    var connectionString = builder.Configuration.GetConnectionString("BoardGameDbContext");
    EnsurePathExistsForSqlLite(connectionString ?? string.Empty);

    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<BoardGameDbContext>();
    await dbContext.Database.EnsureCreatedAsync();

    if (dbContext.BoardGames.Count() == 0)
    {
        await SeedSomeGames(dbContext);
    }
}

app.Run();

static async Task SeedSomeGames(BoardGameDbContext context)
{
    // Create a list of random board game models to seed the db with
    var randomGames = new List<BoardGame>();
    for (var i = 0; i < 100; i++)
    {
        randomGames.Add(BoardGame.ExampleBoardGame());
    }

    await context.BoardGames.AddRangeAsync(randomGames);
    await context.SaveChangesAsync();
}

static void EnsurePathExistsForSqlLite(string connectionString)
{
    if (string.IsNullOrEmpty(connectionString)) return;

    var dataSource = connectionString.Split(new[] { "Data Source=" }, StringSplitOptions.None).LastOrDefault();
    if (string.IsNullOrEmpty(dataSource)) return;

    var directoryPath = Path.GetDirectoryName(dataSource);
    if (string.IsNullOrEmpty(directoryPath)) return;

    Directory.CreateDirectory(directoryPath);
}
