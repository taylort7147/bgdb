using Asp.Versioning;
using BggExt;
using BggExt.Data;
using BggExt.Models;
using BggExt.Services;
using BggExt.Web;
using BggExt.XmlApi2;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;


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
    .AddSingleton<ImageStore>()
    .AddSingleton<LibrarySynchronizer>()
    .AddSingleton<SynchronizationSchedulerService>()
    .AddSingleton<IJobQueue>(ctx =>
    {
        var queueCapacity = 100;
        return new DefaultBackgroundTaskQueue(queueCapacity);
    })
    .AddSingleton<ISynchronizationJobQueue, SynchronizationJobQueue>();

builder.Services
    .AddHostedService<JobQueueProcessorService>()
    .AddHttpClient<Downloader>(c =>
    {
        // Set BaseURL and stuff here, more efficient than making a new client all the time
    })
    .SetHandlerLifetime(TimeSpan.FromMinutes(15));

var connectionString = builder.Configuration.GetConnectionString("BoardGameDbContext") ??
    throw new InvalidOperationException("Connection string 'BoardGameDbContext' not found.");
builder.Services.AddDbContext<BoardGameDbContext>(options =>
{
    options.UseNpgsql(connectionString);
    options.EnableSensitiveDataLogging();
});

builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<BoardGameDbContext>();
builder.Services.AddAuthorization();

var app = builder.Build();
Console.WriteLine($"Using development environment: {app.Environment.IsDevelopment()}");

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
app.MapGroup("api/account").MapIdentityApi<ApplicationUser>();

app.MapFallbackToFile("index.html");

//Could also just add endpoints mapped to the app here rather than using controllers
var versionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1, 0))
    .HasApiVersion(new ApiVersion(2, 0))
    .ReportApiVersions()
    .Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<BoardGameDbContext>();
    await dbContext.Database.MigrateAsync();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await CreateRoleIfNotExists(roleManager, "Administrator");
    await CreateRoleIfNotExists(roleManager, "LibraryOwner");
    await CreateRoleIfNotExists(roleManager, "User");
}

// Must be started after database is initialized
using (var scope = app.Services.CreateScope())
{
    var scheduler = scope.ServiceProvider.GetRequiredService<SynchronizationSchedulerService>();
    await scheduler.StartAsync(app.Lifetime.ApplicationStopped);
}

app.Run();

static async Task CreateRoleIfNotExists(RoleManager<IdentityRole> roleManager, string role)
{
    if (!await roleManager.RoleExistsAsync(role))
    {
        await roleManager.CreateAsync(new IdentityRole() { Name = role });
    }
}
