using BggExt.Models;

namespace BggExt.Services;

public interface ISynchronizationJobQueue
{    
    // TODO: Instead of a delay, make this a time point to proceed
    ValueTask QueueJobAsync(string libraryId, TimeSpan? delay = null);

    ValueTask<Func<CancellationToken, ValueTask>> DequeueJobAsync(CancellationToken cancellationToken);
}
