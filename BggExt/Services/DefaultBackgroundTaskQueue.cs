using System.Threading.Channels;

namespace BggExt.Services;

/// <summary>
/// Default implementation for the IJobQueue.
/// 
/// Adapted from https://learn.microsoft.com/en-us/dotnet/core/extensions/queue-service
/// </summary>
public sealed class DefaultBackgroundTaskQueue : IJobQueue
{
    private readonly Channel<Func<CancellationToken, ValueTask>> _queue;

    public DefaultBackgroundTaskQueue(int capacity)
    {
        BoundedChannelOptions options = new(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait
        };
        _queue = Channel.CreateBounded<Func<CancellationToken, ValueTask>>(options);
    }

    public async ValueTask QueueJobAsync(
        Func<CancellationToken, ValueTask> workItem)
    {
        ArgumentNullException.ThrowIfNull(workItem);

        await _queue.Writer.WriteAsync(workItem);
    }

    public async ValueTask<Func<CancellationToken, ValueTask>> DequeueJobAsync(
        CancellationToken cancellationToken)
    {
        Func<CancellationToken, ValueTask>? workItem =
            await _queue.Reader.ReadAsync(cancellationToken);

        return workItem;
    }
}
