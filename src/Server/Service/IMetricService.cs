using Gates.Shared.Responses;
using Gates.Shared.ServiceModels;

namespace Gates.Server.Service
{
    public interface IMetricService
    {
        public Task<MetricResponse> GetFlaggerStatusForApp(MetricRequest request);
        public Task<MetricResponse> GetIsAppLive(MetricRequest request);
    }
}
