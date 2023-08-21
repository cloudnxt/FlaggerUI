using AutoMapper;
using Gates.Server.Data;
using Gates.Shared.Data;
using Microsoft.EntityFrameworkCore;

namespace Gates.Server.Service
{
    public class CanaryService : ICanaryService
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IGateService _gateService;
        private readonly IEventService _eventService;

        public CanaryService(AppDbContext dbContext, IMapper mapper, IGateService gateService, IEventService eventService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _gateService = gateService;
            _eventService = eventService;
        }

        public async Task<List<CanaryModel>> GetAllCanaries()
        {
            return await _dbContext.Canaries.ToListAsync();
        }

        public async Task<CanaryModel> GetCanaryByAppId(int appId)
        {
            return await _dbContext.Canaries.Where(c => c.AppId == appId).FirstOrDefaultAsync();
        }

        public async Task<int> CreateCanary(CanaryModel canary)
        {
            _dbContext.Canaries.Add(canary);
            await _dbContext.SaveChangesAsync();
            AddAppEvent(canary);
            return canary.Id;
        }

        public async Task<bool> DeleteCanary(int appId)
        {
            var canary = await _dbContext.Canaries.Where(c => c.AppId == appId).FirstOrDefaultAsync();
            if (canary == null)
                return false;

            RemoveAppEvent(canary);

            _dbContext.Canaries.Remove(canary);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<AppModel> GetCanaryByNameAndSpace(string app, string space)
        {
            return await _dbContext.Apps.Where(a => a.Name == app && a.Namespace == space).FirstOrDefaultAsync();
        }


        private void AddAppEvent(CanaryModel request)
        {
            var model = _mapper.Map<EventModel>(request);
            model.Id = 0;
            model.Phase = "Apply Canary Resource";
            model.EventMessage = "Registered";
            _eventService.CreateEvent(model);
        }

        private void RemoveAppEvent(CanaryModel request)
        {
            var model = _mapper.Map<EventModel>(request);
            model.Id = 0;
            model.Phase = "Remove Canary";
            model.EventMessage = "De-Registered";
            _eventService.CreateEvent(model);
        }
    }
}
