using Gates.Server.Data;
using Gates.Shared.Data;

namespace Gates.Server.Service
{
    public interface IEventService
    {
        Task<List<EventModel>> GetAllEvents();
      
        Task<int> CreateEvent(EventModel model);

        Task<List<EventModel>> GetEvents(string name, string _namespace);
    }
}
