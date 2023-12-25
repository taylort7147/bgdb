namespace BggExt.Services;

/// <summary>
/// This is a hosted services that continuously processes jobs from a queue 
/// until stopped.
/// 
/// Adapted from https://learn.microsoft.com/en-us/dotnet/core/extensions/queue-service
/// </summary>
/// <param name="_jobQueue"></param>
/// <param name="logger"></param>
public sealed class JobQueueProcessorService(
        IJobQueue _jobQueue,
        ILogger<JobQueueProcessorService> logger) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation($"{nameof(JobQueueProcessorService)} is running.");
        return ProcessTaskQueueAsync(stoppingToken);
    }

    private async Task ProcessTaskQueueAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var job = await _jobQueue.DequeueJobAsync(stoppingToken);
                await job(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Prevent throwing if stoppingToken was signaled
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred executing task work item.");
            }
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation($"{nameof(JobQueueProcessorService)} is stopping.");
        await base.StopAsync(stoppingToken);
    }
}
