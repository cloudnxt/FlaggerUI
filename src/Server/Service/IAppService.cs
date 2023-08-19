using Gates.Server.Data;
using Gates.Shared.Data;

namespace Gates.Server.Service
{
    public interface IAppService
    {
        Task<List<AppModel>> GetAllApps();
        Task<AppModel> GetAppById(int appId);
        Task<int> CreateApp(AppModel app);
        Task<bool> UpdateApp(AppModel app);
        Task<bool> DeleteApp(int appId);

        Task<AppModel> GetAppNameAndSpace(string app, string space);
    }
}
