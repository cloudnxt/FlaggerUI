using Gates.Shared.Data;
using Gates.Shared.Requests;
using System.Threading.Channels;

namespace Gates.Server.Service
{
    public sealed class DefaultBackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly Channel<LoadTestModel> _queue;

        public DefaultBackgroundTaskQueue(int capacity)
        {
            BoundedChannelOptions options = new(capacity)
            {
                FullMode = BoundedChannelFullMode.Wait
            };
            _queue = Channel.CreateBounded<LoadTestModel>(options);
        }

        public async Task QueueBackgroundWorkItemAsync(
            LoadTestModel workItem)
        {
            if (workItem is null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            await _queue.Writer.WriteAsync(workItem);
        }

        public async Task<LoadTestModel> DequeueAsync(
            CancellationToken cancellationToken)
        {
            LoadTestModel workItem =
                await _queue.Reader.ReadAsync(cancellationToken);

            return workItem;
        }
    }
}
