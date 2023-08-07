using Gates.Server.Service;
using Gates.Shared.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Gates.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoadTestLogsController : ControllerBase
    {
        private readonly ILogsServices _logService;

        public LoadTestLogsController(ILogsServices logService)
        {
            _logService = logService;
        }


        // GET: api/app/{appId}
        [HttpGet("{Id}")]
        public async Task<ActionResult<AppModel>> GetAppById(int Id)
        {
            var app = await _logService.GetLogByLoadTest(Id);
            if (app == null)
                return NotFound();

            return Ok(app);
        }
    }
}
