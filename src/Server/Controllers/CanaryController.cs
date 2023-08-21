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
        private readonly ICanaryService _canaryService;
        private readonly IEventService _eventService;

        public CanaryController(IAppService appService, IMapper mapper, ICanaryService canaryService, IEventService eventService, ILogger<AppController> logger)
        {
            _appService = appService;
            _mapper = mapper;
            _canaryService = canaryService;
            _eventService = eventService;
        }

        // GET: api/app
        [HttpGet]
        public async Task<ActionResult<List<AppModel>>> GetAllApps()
        {
            var canaries = await _canaryService.GetAllCanaries();
            return Ok(canaries);
        }

        [HttpGet("{appId}")]
        public async Task<ActionResult<AppModel>> Get(int appId)
        {
            var canary = await _canaryService.GetCanaryByAppId(appId);
            return Ok(canary);
        }
    }
}
