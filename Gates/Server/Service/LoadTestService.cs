using Gates.Server.Data;
using Gates.Shared.Data;
using Gates.Shared.Enums;
using Gates.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Gates.Server.Service
{
    public class LoadTestService : ILoadTestService
    {
        private readonly AppDbContext _dbContext;

        public LoadTestService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<LoadTestModel>> GetAll()
        {
            return await _dbContext.LoadTestRuns.ToListAsync();
        }
        public async Task<LoadTestModel> GetLoadTestResponseById(int Id)
        {
            return await _dbContext.LoadTestRuns.FindAsync(Id);
        }

        public async Task<RunLoadTestApiResponse> CreateTestRun(LoadTestModel testRun)
        {
            _dbContext.LoadTestRuns.Add(testRun);
            await _dbContext.SaveChangesAsync();
            return new RunLoadTestApiResponse() { Status = "Test Run Initiated", TestRunId = testRun.Id};
        }

        public async Task UpdateStatusCompleted(LoadTestModel model)
        {
            model.Completed = true;
            _dbContext.LoadTestRuns.Update(model);
            await _dbContext.SaveChangesAsync();
        }
    }
}
