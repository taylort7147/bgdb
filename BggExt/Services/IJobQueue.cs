namespace BggExt.Services;

/// <summary>
/// Interface for a simple job queue.
/// 
/// Adapted from https://learn.microsoft.com/en-us/dotnet/core/extensions/queue-service
/// </summary>
public interface IJobQueue
{
    ValueTask QueueJobAsync(Func<CancellationToken, ValueTask> job);

    ValueTask<Func<CancellationToken, ValueTask>> DequeueJobAsync(CancellationToken cancellationToken);
}
