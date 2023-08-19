using Gates.Shared.Data;
using Gates.Shared.Responses;

namespace Gates.Server.Service
{
    public interface ILoadTestService
    {
        Task<List<LoadTestModel>> GetAll();
        Task<LoadTestModel> GetLoadTestResponseById(int Id);
        Task<RunLoadTestApiResponse> CreateTestRun(LoadTestModel testRun);
        Task UpdateStatusCompleted(LoadTestModel model);
    }
}
