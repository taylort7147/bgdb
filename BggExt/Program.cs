using Asp.Versioning;
using BggExt;
using BggExt.Data;
using BggExt.Models;
using BggExt.Web;
using BggExt.XmlApi2;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;
using BoardGame = BggExt.Models.BoardGame;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Options;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(opts =>
        opts.ModelMetadataDetailsProviders.Add(new SystemTextJsonValidationMetadataProvider())
    )
    .AddJsonOptions(opts =>
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

builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<BoardGameDbContext>();
builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BoardGameDbContext>();
    db.Database.Migrate();
}

if (!app.Environment.IsDevelopment())
    app.UseHsts();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseConfiguredSwaggerUI();


app.MapControllerRoute(
    "default",
    "{controller}/{action=Index}/{id?}");
// Place Identity routing after controller routing because Swagger is 
// configured for "first-come-first-serve" strategy for API routes.
app.MapGroup("/account").MapIdentityApi<ApplicationUser>(); 

app.MapFallbackToFile("index.html");

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
    EnsurePathExistsForSqlite(connectionString ?? string.Empty);

    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<BoardGameDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
        if (dbContext.BoardGames.Count() == 0)
        {
            await SeedSomeGames(dbContext);
        }

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        await CreateRoleIfNotExists(roleManager, "Administrator");
        await CreateRoleIfNotExists(roleManager, "LibraryOwner");
        await CreateRoleIfNotExists(roleManager, "User");
    }

}

app.Run();

static async Task CreateRoleIfNotExists(RoleManager<IdentityRole> roleManager, string role)
{
    if (!await roleManager.RoleExistsAsync(role))
    {
        await roleManager.CreateAsync(new IdentityRole() { Name = role });
    }
}

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

static void EnsurePathExistsForSqlite(string connectionString)
{
    if (string.IsNullOrEmpty(connectionString)) return;

    var dataSource = connectionString.Split(new[] { "Data Source=" }, StringSplitOptions.None).LastOrDefault();
    if (string.IsNullOrEmpty(dataSource)) return;

    var directoryPath = Path.GetDirectoryName(dataSource);
    if (string.IsNullOrEmpty(directoryPath)) return;

    Directory.CreateDirectory(directoryPath);
}
