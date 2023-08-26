using AutoMapper;
using Gates.Client;
using Gates.Server.Data;
using Gates.Shared.Data;
using Gates.Shared.Enums;
using Gates.Shared.Requests;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text.Json;
using System.Threading.Channels;

namespace Gates.Server.Service
{
    public class AppService : IAppService
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IGateService _gateService;
        private readonly IEventService _eventService;

        public AppService(AppDbContext dbContext, IMapper mapper, IGateService gateService, IEventService eventService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _gateService = gateService;
            _eventService = eventService;
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

            AddAppEvent(app);
            CreateAppGates(app);

            return app.Id;
        }

        public async Task<bool> UpdateApp(AppModel newApp)
        {
            var existingApp = await GetAppNameAndSpace(newApp.Name, newApp.Namespace);
            if (existingApp == null)
                return false;
            // Update only if Image is not same.
            if (existingApp.Image != newApp.Image)
            {
                if (String.IsNullOrWhiteSpace(existingApp.OldImages))
                {
                    var changed = new ImageDetails() { Image = existingApp.Image, Applied = existingApp.Updated };
                    var changes = new List<ImageDetails>();
                    changes.Add(changed);
                    newApp.OldImages = JsonSerializer.Serialize(changes);
                }
                else
                {
                    var imagesUsed = JsonSerializer.Deserialize<List<ImageDetails>>(existingApp.OldImages);
                    imagesUsed.Add(new ImageDetails() { Image = existingApp.Image, Applied = existingApp.Updated });
                    newApp.OldImages = JsonSerializer.Serialize(imagesUsed);

                }
            }
            else
            {
                newApp.OldImages = existingApp.OldImages;
            }
            existingApp.Image = newApp.Image;
            existingApp.ContainerPorts = newApp.ContainerPorts;
            existingApp.Replicas = newApp.Replicas;
            existingApp.OldImages = newApp.OldImages;
            existingApp.Updated = DateTime.Now;
            _dbContext.Update(existingApp);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteApp(int appId)
        {
            var app = await _dbContext.Apps.FindAsync(appId);
            if (app == null)
                return false;

            _gateService.RemoveGate(appId);
            RemoveAppEvent(app);

            _dbContext.Apps.Remove(app);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<AppModel> GetAppNameAndSpace(string app, string space)
        {
            return await _dbContext.Apps.Where(a => a.Name == app && a.Namespace == space).FirstOrDefaultAsync();
        }


        private void AddAppEvent(AppModel request)
        {
            var model = _mapper.Map<EventModel>(request);
            model.Id = 0;
            model.Phase = "Registered";
            model.EventMessage = "New App Registered";
            _eventService.CreateEvent(model);
        }

        private void RemoveAppEvent(AppModel request)
        {
            var model = _mapper.Map<EventModel>(request);
            model.Id = 0;
            model.Phase = "De-Registered";
            model.EventMessage = $"App Removed";
            _eventService.CreateEvent(model);
        }

        private void CreateAppGates(AppModel request)
        {
            foreach (var field in typeof(WebhookStateEnum).GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                _gateService.AddGate(new GateModel()
                {
                    AppId = request.Id,
                    Name = request.Name,
                    Namespace = request.Namespace,
                    WebhookState = field.Name.ToString(),
                    Status = GateStatusEnum.Close.ToString()
                });
            }
        }
    }
}
