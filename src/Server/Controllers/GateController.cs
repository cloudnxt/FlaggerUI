using AutoMapper;
using Gates.Server.Service;
using Gates.Shared.Data;
using Gates.Shared.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Gates.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GateController : ControllerBase
    {
        private readonly IGateService _gateService;
        private readonly IMapper _mapper;
        private readonly ILogger<GateController> _logger;

        public GateController(IGateService gateService, IMapper mapper, ILogger<GateController> logger)
        {
            _gateService = gateService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string webhookState)
        {
            var events = await _gateService.GetGate(webhookState);
            return Ok(events);
        }


        [HttpPost]
        public IActionResult AddGate([FromBody] GateApiRequest model)
        {
            var gate = _mapper.Map<GateModel>(model);
            bool isSuccess = _gateService.AddGate(gate);
            if (isSuccess)
                return Ok("Gate added successfully.");
            else
                return BadRequest("Failed to add gate.");
        }

        [HttpDelete]
        public IActionResult RemoveGate([FromBody] GateModel model)
        {
            bool isSuccess = _gateService.RemoveGate(model);
            if (isSuccess)
                return Ok("Gate removed successfully.");
            else
                return BadRequest("Failed to remove gate.");
        }

        [HttpPut]
        public IActionResult UpdateGate([FromBody] GateModel model)
        {
            _gateService.UpdateGate(model);
            return Ok("Gate updated successfully.");
        }

        [HttpPost("Open")]
        public IActionResult OpenGate([FromBody] GateModel model)
        {
            _gateService.OpenGate(model);
            return Ok("Gate opened successfully.");
        }

        [HttpPost("Close")]
        public IActionResult CloseGate([FromBody] GateModel model)
        {
            _gateService.CloseGate(model);
            return Ok("Gate closed successfully.");
        }

        [HttpPost("Check")]
        public async Task<IActionResult> CheckGate([FromBody] GateApiRequest model)
        {
            _logger.LogInformation($"Checking :  {JsonSerializer.Serialize(model)}");

            var gate = await _gateService.GetGate(model.Name, model.Namespace, model.Metadata.WebhookState);

            if (gate == null)
            {
                gate = _mapper.Map<GateModel>(model);
                _gateService.AddGate(gate);
            }
            
            var check = await _gateService.CheckGate(gate);

            if (check)
            {
                _gateService.ResetWaiting(gate);
                return Ok("Gate is open");
            }
            else
            {

                _gateService.UpdateIsWaiting(gate);
                return BadRequest("Gate is Closed");
            }
        }
    }
}
