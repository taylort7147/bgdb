using BggExt.Models;

namespace BggExt.Services;

/// <summary>
/// Implementation for ISynchronizationJobQueue which wraps an IJobQueue.
/// </summary>
/// <param name="_jobQueue"></param>
/// <param name="_librarySynchronizer"></param>
public class SynchronizationJobQueue(
    IJobQueue _jobQueue,
    LibrarySynchronizer _librarySynchronizer) : ISynchronizationJobQueue
{
    public async ValueTask QueueJobAsync(string libraryId, TimeSpan? delay = null)
    {
        await _jobQueue.QueueJobAsync(BuildJobObject(libraryId, delay));
    }

    public async ValueTask<Func<CancellationToken, ValueTask>> DequeueJobAsync(CancellationToken cancellationToken)
    {
        return await _jobQueue.DequeueJobAsync(cancellationToken);
    }

    private Func<CancellationToken, ValueTask> BuildJobObject(string libraryId, TimeSpan? delay)
    {
        return async token =>
        {
            if (delay != null)
            {
                await Task.Delay(delay.Value);
            }
            var status = await _librarySynchronizer.Synchronize(token, libraryId);

            // If pending, add it back to the queue with a delay
            if(status == LibrarySynchronizer.Status.Pending)
            {
                await QueueJobAsync(libraryId, TimeSpan.FromSeconds(30));
            }
        };
    }
}
