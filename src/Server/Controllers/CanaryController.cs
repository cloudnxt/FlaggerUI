using AutoMapper;
using Gates.Server.Service;
using Gates.Shared.Data;
using Microsoft.AspNetCore.Mvc;

namespace Gates.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CanaryController : ControllerBase
    {
        private readonly IAppService _appService;
        private readonly IMapper _mapper;
        private readonly IGateService _gateService;
        private readonly IEventService _eventService;

        public CanaryController(IAppService appService, IMapper mapper, IGateService gateService, IEventService eventService, ILogger<AppController> logger)
        {
            _appService = appService;
            _mapper = mapper;
            _gateService = gateService;
            _eventService = eventService;
        }

        // GET: api/app
        [HttpGet]
        public async Task<ActionResult<List<AppModel>>> GetAllApps()
        {
            var apps = await _appService.GetAllApps();
            return Ok(apps);
        }
    }
}
