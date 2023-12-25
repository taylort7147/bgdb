using System.Dynamic;
using BggExt.Data;
using BggExt.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BggExt.Services;

/// <summary>
/// This is a background service that schedules library synchronization for all
/// enabled libraries on a regular basis.
/// 
/// Adapted from https://learn.microsoft.com/en-us/dotnet/core/extensions/queue-service
/// </summary>
/// <param name="_jobQueue"></param>
/// <param name="_logger"></param>
/// <param name="_serviceProvider"></param>
public sealed class SynchronizationSchedulerService(
    ISynchronizationJobQueue _jobQueue,
    ILogger<SynchronizationSchedulerService> _logger,
    IServiceProvider _serviceProvider) : IHostedService, IDisposable
{
    private Timer? _timer = null;

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Timed Hosted Service running.");

        _timer = new Timer(DoWork, null, TimeSpan.Zero,
            TimeSpan.FromHours(12));

        return Task.CompletedTask;
    }

    private async void DoWork(object? state)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<BoardGameDbContext>();
            var libraries = await context.Libraries.ToListAsync();
            foreach (var library in libraries)
            {
                await _jobQueue.QueueJobAsync(library.Id);
            }
        }
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"{nameof(SynchronizationSchedulerService)} is stopping.");

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
