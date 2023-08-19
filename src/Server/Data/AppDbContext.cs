using Gates.Shared.Data;
using Microsoft.EntityFrameworkCore;

namespace Gates.Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<AppModel> Apps { get; set; }
        public DbSet<GateModel> Gates { get; set; }
        public DbSet<EventModel> Events { get; set; }
        public DbSet<LoadTestModel> LoadTestRuns { get; set; }
        public DbSet<LoadTestLogModel> LoadTestLogs { get; set; }
        
    }

}
