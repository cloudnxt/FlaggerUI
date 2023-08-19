using Gates.Client;
using Gates.Server.Data;
using Gates.Shared.Data;
using Microsoft.EntityFrameworkCore;

namespace Gates.Server.Service
{
    public class AppService : IAppService
    {
        private readonly AppDbContext _dbContext;

        public AppService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<AppModel>> GetAllApps()
        {
            return await _dbContext.Apps.ToListAsync();
        }

        public async Task<AppModel> GetAppById(int appId)
        {
            return await _dbContext.Apps.FindAsync(appId);
        }

        public async Task<int> CreateApp(AppModel app)
        {
            _dbContext.Apps.Add(app);
            await _dbContext.SaveChangesAsync();
            return app.Id;
        }

        public async Task<bool> UpdateApp(AppModel app)
        {
            _dbContext.Entry(app).State = EntityState.Modified;
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteApp(int appId)
        {
            var app = await _dbContext.Apps.FindAsync(appId);
            if (app == null)
                return false;

            _dbContext.Apps.Remove(app);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<AppModel> GetAppNameAndSpace(string app, string space)
        {
            return await _dbContext.Apps.Where(a=>a.Name == app && a.Namespace == space).FirstOrDefaultAsync();
        }
    }
}
