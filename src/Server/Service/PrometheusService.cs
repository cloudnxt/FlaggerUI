using Gates.Shared.Responses;
using Gates.Shared.ServiceModels;
using Microsoft.Extensions.Logging;
using static System.Net.WebRequestMethods;

namespace Gates.Server.Service
{
    public class PrometheusService : IMetricService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<PrometheusService> _logger;
        private readonly HttpClient _httpClient;
        private bool IsMetricServerReady { get; set; }
        public PrometheusService(IHttpClientFactory httpClientFactory, ILogger<PrometheusService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _httpClient = _httpClientFactory.CreateClient();
            var address = Environment.GetEnvironmentVariable("METRIC_SERVER");
            if (address == null)
                IsMetricServerReady = false;
            else
                _httpClient.BaseAddress = new Uri(address);
        }



        public async Task<MetricResponse> GetFlaggerStatusForApp(MetricRequest request)
        {
            var result = await _httpClient.GetAsync("api/v1/query?query=flagger_canary_status{name=\"" + request.appname + "\",namespace=\"" + request.Namespace + "\"}");
            var res = await result.Content.ReadFromJsonAsync<MetricResponse>();

            if (result.IsSuccessStatusCode)
            {
                return res;
            }

            return null;
        }

        public Task<MetricResponse> GetIsAppLive(MetricRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
