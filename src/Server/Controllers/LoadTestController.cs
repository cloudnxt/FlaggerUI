using AutoMapper;
using Gates.Server.Service;
using Gates.Shared.Data;
using Gates.Shared.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Gates.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoadTestController : ControllerBase
    {

        private readonly ILoadTestService _loadTestService;
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly IMapper _mapper;
        private readonly ILogger<LoadTestController> _logger;

        public LoadTestController(ILoadTestService loadTestService, IMapper mapper, IBackgroundTaskQueue taskQueue, ILogger<LoadTestController> logger)
        {
            _loadTestService = loadTestService;
            _mapper = mapper;
            _taskQueue = taskQueue;
            _logger = logger;
        }


        [HttpPost]
        public async Task<IActionResult> Post(RunLoadTestApiRequest request)
        {
            var model = _mapper.Map<LoadTestModel>(request);
            var response = _loadTestService.CreateTestRun(model);
            await _taskQueue.QueueBackgroundWorkItemAsync(model);
            return Ok(response);
        }



        [HttpGet("{Id}")]
        public async Task<IActionResult> Get([FromQuery] int Id)
        {
                return Ok(await _loadTestService.GetLoadTestResponseById(Id));

        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _loadTestService.GetAll());

        }
    }
}
