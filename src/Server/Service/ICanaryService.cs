using Gates.Shared.Data;

namespace Gates.Server.Service
{
    public interface ICanaryService
    {
        Task<int> CreateCanary(CanaryModel canary);
        Task<bool> DeleteCanary(int appId);
        Task<List<CanaryModel>> GetAllCanaries();
        Task<CanaryModel> GetCanaryByAppId(int appId);
        Task<AppModel> GetCanaryByNameAndSpace(string app, string space);
    }
}