using Gates.Shared.Data;
using Gates.Shared.Requests;

namespace Gates.Server.Service
{
    public interface IBackgroundTaskQueue
    {
        Task QueueBackgroundWorkItemAsync(LoadTestModel workItem);

        Task<LoadTestModel> DequeueAsync(CancellationToken cancellationToken);
    }
}
