using BggExt.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<BoardGameDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("BoardGameDbContext") ?? throw new InvalidOperationException("Connection string 'BoardGameDbContext' not found.")));}
else
{
    builder.Services.AddDbContext<BoardGameDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("ProductionBoardGameDbContext")));
}
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();