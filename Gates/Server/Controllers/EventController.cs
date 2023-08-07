using AutoMapper;
using Gates.Server.Service;
using Gates.Shared.Data;
using Gates.Shared.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Gates.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IMapper _mapper;
       

        public EventController(IEventService eventService, IMapper mapper)
        {
            _eventService = eventService;
            _mapper = mapper;
        }

        [HttpPost]
        public IActionResult Post(AddEventApiRequest request)
        {
            var model = _mapper.Map<EventModel>(request);
            _eventService.CreateEvent(model);
            return Ok(model);
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string? name, [FromQuery] string? _namespace)
        {
            if (name == null || _namespace == null)
            {
                return Ok(await _eventService.GetAllEvents());
            }
            
            return Ok(await _eventService.GetEvents(name, _namespace));
        }

    }
}
