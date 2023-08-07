using Gates.Shared.Data;
using Gates.Shared.Requests;
using Microsoft.VisualBasic;

namespace Gates.Server.Service
{
    public sealed class QueuedHostedService : BackgroundService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly ILogger<QueuedHostedService> _logger;
        private readonly IServiceProvider _services;

        public QueuedHostedService(
            IHttpClientFactory httpClientFactory,
            IBackgroundTaskQueue taskQueue,
            ILogger<QueuedHostedService> logger,
            IServiceProvider services
            ) =>
            (_httpClientFactory, _taskQueue, _logger, _services) = (httpClientFactory, taskQueue, logger, services);

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                $"{nameof(QueuedHostedService)} is running.{Environment.NewLine}" +
                $"{Environment.NewLine}Tap W to add a work item to the " +
                $"background queue.{Environment.NewLine}");

            return ProcessTaskQueueAsync(stoppingToken);
        }

        private async Task ProcessTaskQueueAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _services.CreateScope())
                    {
                        var _loadTestService = scope.ServiceProvider.GetRequiredService<ILoadTestService>();
                        var _loadTestLogsService = scope.ServiceProvider.GetRequiredService<ILogsServices>();
                        LoadTestModel? workItem =
                        await _taskQueue.DequeueAsync(stoppingToken);

                        var iteration = 0;
                        while (!stoppingToken.IsCancellationRequested && iteration < workItem.NoOfRequests)
                        {
                            try
                            {
                                var client = _httpClientFactory.CreateClient();
                                var response = await client.GetAsync(workItem.Url);
                                if (response.IsSuccessStatusCode)
                                {
                                    await _loadTestLogsService.Create(new LoadTestLogModel()
                                    {
                                        LoadTestId = workItem.Id,
                                        HttpStatus = response.StatusCode.ToString(),
                                        Url = workItem.Url,
                                        Response = await response.Content.ReadAsStringAsync()
                                    });
                                }
                                else
                                {
                                    await _loadTestLogsService.Create(new LoadTestLogModel()
                                    {
                                        LoadTestId = workItem.Id,
                                        HttpStatus = response.StatusCode.ToString(),
                                        Url = workItem.Url,
                                        Response = await response.Content.ReadAsStringAsync()
                                    });
                                }

                                _logger.LogInformation($"Trying iteration {iteration}");
                                iteration++;
                            }
                            catch (OperationCanceledException e)
                            {
                                iteration++;
                                await _loadTestLogsService.Create(new LoadTestLogModel()
                                {
                                    LoadTestId = workItem.Id,
                                    HttpStatus = "Error Making Request",
                                    Url = workItem.Url,
                                    Response = e.Message
                                }); // Prevent throwing if the Delay is cancelled
                            }
                            catch (Exception ex)
                            {
                                iteration++;
                                await _loadTestLogsService.Create(new LoadTestLogModel()
                                {
                                    LoadTestId = workItem.Id,
                                    HttpStatus = "Error Making Request",
                                    Url = workItem.Url,
                                    Response = ex.Message
                                });
                            }
                            _logger.LogInformation($"Queued work item is running {workItem.NoOfRequests}");

                        }


                        await _loadTestService.UpdateStatusCompleted(workItem);
                    }
                }
                catch (OperationCanceledException)
                {
                    // Prevent throwing if stoppingToken was signaled
                }
                catch (Exception ex)
                {

                    _logger.LogError(ex, "Error occurred executing task work item.");
                }
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                $"{nameof(QueuedHostedService)} is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}
