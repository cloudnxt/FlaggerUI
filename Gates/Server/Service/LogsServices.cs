using Gates.Server.Data;
using Gates.Shared.Data;
using Gates.Shared.Responses;
using Microsoft.EntityFrameworkCore;

namespace Gates.Server.Service
{
    public class LogsServices : ILogsServices
    {

        private readonly AppDbContext _dbContext;

        public LogsServices(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<LoadTestLogModel>> GetAll()
        {
            return await _dbContext.LoadTestLogs.ToListAsync();
        }
        public async Task<List<LoadTestLogModel>> GetLogByLoadTest(int Id)
        {
            return await _dbContext.LoadTestLogs.Where(l => l.LoadTestId == Id).ToListAsync();
        }

        public async Task Create(LoadTestLogModel log)
        {
            _dbContext.LoadTestLogs.Add(log);
            await _dbContext.SaveChangesAsync();
        }
    }
}
