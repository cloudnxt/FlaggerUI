﻿using AutoMapper;
using Gates.Server.Service;
using Gates.Shared.Data;
using Gates.Shared.Enums;
using Gates.Shared.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Gates.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppController : ControllerBase
    {
        private readonly IAppService _appService;
        private readonly IMapper _mapper;
        private readonly IGateService _gateService;
        private readonly IEventService _eventService;

        public AppController(IAppService appService, IMapper mapper, IGateService gateService, IEventService eventService, ILogger<AppController> logger)
        {
            _appService = appService;
            _mapper = mapper;
            _gateService = gateService;
            _eventService = eventService;
        }

        [HttpGet]
        public async Task<ActionResult<List<AppModel>>> GetAllApps()
        {
            var apps = await _appService.GetAllApps();
            return Ok(apps);
        }

        [HttpGet("{appId}")]
        public async Task<ActionResult<AppModel>> GetAppById(int appId)
        {
            var app = await _appService.GetAppById(appId);
            if (app == null)
                return NotFound();

            return Ok(app);
        }

        [HttpPost]
        public async Task<ActionResult<int>> CreateApp(AddAppApiRequest request)
        {
            var app = await _appService.GetAppNameAndSpace(request.Name, request.Namespace);
            if (app == null)
            {
                var appModel = _mapper.Map<AppModel>(request);
                var appId = await _appService.CreateApp(appModel);
                return CreatedAtAction(nameof(GetAppById), new { appId }, appId);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost("Disable")]
        public async Task<ActionResult<int>> DisableAllGates(AddAppApiRequest request)
        {
            var app = await _appService.GetAppNameAndSpace(request.Name, request.Namespace);
            if (app != null)
            {
                _gateService.ModifyAllGatesStatus(app.Id, GateStatusEnum.Close);
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }


        [HttpPost("Enable")]
        public async Task<ActionResult<int>> EnableAllGates(AppModel request)
        {
            var app = await _appService.GetAppNameAndSpace(request.Name, request.Namespace);
            if (app != null)
            {
                _gateService.ModifyAllGatesStatus(app.Id, GateStatusEnum.Open);
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        // PUT: api/app/{appId}
        [HttpPut("{appId}")]
        public async Task<IActionResult> UpdateApp(int appId, AppModel app)
        {
            if (appId != app.Id)
                return BadRequest();

            var success = await _appService.UpdateApp(app);
            if (!success)
                return NotFound();

            return NoContent();
        }

        // DELETE: api/app/{appId}
        [HttpDelete("{appId}")]
        public async Task<IActionResult> DeleteApp(int appId)
        {
            var success = await _appService.DeleteApp(appId);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}
