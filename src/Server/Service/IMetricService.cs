using Gates.Shared.ServiceModels;

namespace Gates.Server.Service
{
    public interface IMetricService
    {
        public Task<string> GetFlaggerStatusForApp(MetricRequest request);
        public Task<string> GetIsAppLive(MetricRequest request);
    }
}
