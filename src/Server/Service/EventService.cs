using Gates.Server.Data;
using Gates.Shared.Data;
using Microsoft.EntityFrameworkCore;

namespace Gates.Server.Service
{
    public class EventService : IEventService
    {
        private readonly AppDbContext _dbContext;

        public EventService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<EventModel>> GetAllEvents()
        {
            return await _dbContext.Events.ToListAsync();
        }

        public async Task<int> CreateEvent(EventModel model)
        {
            _dbContext.Events.Add(model);
            await _dbContext.SaveChangesAsync();
            return model.Id;
            
        }

        public async Task<List<EventModel>> GetEvents(string name, string _namespace)
        {
            return await _dbContext.Events.Where(e => e.Name ==  name && e.Namespace == _namespace).ToListAsync();
        }
    }
}
