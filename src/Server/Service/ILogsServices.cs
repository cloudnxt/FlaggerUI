using Gates.Shared.Data;

namespace Gates.Server.Service
{
    public interface ILogsServices
    {
        Task Create(LoadTestLogModel log);
        Task<List<LoadTestLogModel>> GetAll();
        Task<List<LoadTestLogModel>> GetLogByLoadTest(int Id);
    }
}