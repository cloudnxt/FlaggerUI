using AutoMapper;
using Gates.Server.Service;
using Gates.Shared.Data;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

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
            if (canary != null)
            {
                return Ok(canary);
            }
            return NotFound();
        }

        [HttpGet("downloadcanary/{appId}")]
        public IActionResult DownloadFile(int appId)
        {
            string filePath = @"canary.yaml";
            try
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

                Console.WriteLine("File read and converted to bytes successfully.");

                // You can now use the 'fileBytes' array as needed.
                var contentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = "canary.yaml"
                };
                Response.Headers.Add("Content-Disposition", contentDisposition.ToString());

                return File(fileBytes, "application/octet-stream");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            return NotFound(filePath);
        }
    }
}
