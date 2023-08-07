using AutoMapper;
using Gates.Server.Service;
using Gates.Shared.Data;
using Gates.Shared.Enums;
using Gates.Shared.Requests;
using Gates.Shared.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;

namespace Gates.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdmissionWebhookController : ControllerBase
    {
        private readonly IAppService _appService;
        private readonly IMapper _mapper;
        private readonly IGateService _gateService;
        private readonly IEventService _eventService;
        private readonly ILogger<AppController> _logger;
        private readonly string requiredAnnotation = "enable-canary-gates";
        public AdmissionWebhookController(IAppService appService, IMapper mapper, IGateService gateService, IEventService eventService, ILogger<AppController> logger)
        {
            _appService = appService;
            _mapper = mapper;
            _gateService = gateService;
            _eventService = eventService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<AdmissionResponse> Post([FromBody] AdmissionReview review)
        {
            _logger.LogInformation(JsonSerializer.Serialize(review));
            var operation = review?.Request?.Operation;
            var kind = review?.Request?.Kind?.Kind;
            if (String.Equals(kind, "deployment", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation($"Found Deployment");
                try
                {
                    switch (operation)
                    {
                        case "CREATE":
                            _logger.LogInformation("in CREATE");
                            var annotationExists = review?.Request?.Object?.Metadata?.Annotations?[requiredAnnotation];
                            if (annotationExists == "true")
                            {
                                var space = review?.Request?.Object?.Metadata?.Namespace;
                                var app = review?.Request?.Object?.Metadata?.Name;
                                if (!app.EndsWith("-primary") && !app.EndsWith("-canary"))
                                {
                                    var containers = review?.Request?.Object?.spec?.template?.spec?.containers.Where(c => c.name != "kuma-sidecar");
                                    var images = string.Join(", ", containers?.Select(c => c.image));
                                    var ports = containers?.SelectMany(c => c.ports);
                                    var port_string = string.Join(", ", ports.Select(p => $"{p.containerPort}/{p.protocol}"));
                                    var replicas = review?.Request?.Object?.spec?.replicas;

                                    var exists = await _appService.GetAppNameAndSpace(app, space);
                                    if (exists == null)
                                    {
                                        AddAppApiRequest request = new AddAppApiRequest();
                                        request.Name = app;
                                        request.Namespace = space;
                                        request.Phase = "Registering";
                                        request.Url = $"{app}.{space}.svc";
                                        request.Image = images;
                                        request.Replicas = replicas;
                                        request.ContainerPorts = port_string;
                                        var appModel = _mapper.Map<AppModel>(request);
                                        await _appService.CreateApp(appModel);
                                        AddEvent(request, "Registered");
                                        CreateGates(appModel);
                                    }
                                }
                            }
                            break;

                        case "DELETE":
                            _logger.LogInformation("in DELETE");
                            var oldAnnotation = review?.Request?.OldObject?.Metadata?.Annotations?[requiredAnnotation];
                            if (oldAnnotation == "true")
                            {
                                var space = review?.Request?.OldObject?.Metadata?.Namespace;
                                var app = review?.Request?.OldObject?.Metadata?.Name;
                                if (!app.EndsWith("-primary") && !app.EndsWith("-canary"))
                                {
                                    var exists = await _appService.GetAppNameAndSpace(app, space);
                                    if (exists != null)
                                    {
                                        AddAppApiRequest request = new AddAppApiRequest();
                                        request.Name = app;
                                        request.Namespace = space;
                                        request.Phase = "De-registering";
                                        request.Url = $"{app}.{space}.svc";
                                        await _appService.DeleteApp(exists.Id);
                                        AddEvent(request, "Deregistered");
                                        var appGates = await _gateService.GetGateByAppId(exists.Id);
                                        DeleteGates(appGates);
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }

                catch (KeyNotFoundException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                // Add App here
                return GenerateResponse(review, true);
            }

            else
            {
                return GenerateResponse(review, true);
            }
        }

        private static AdmissionResponse GenerateResponse(AdmissionReview? review, bool allowed)
        {
            // Create the AdmissionResponse object
            return new AdmissionResponse
            {
                ApiVersion = "admission.k8s.io/v1",
                Kind = "AdmissionReview",
                Response = new AdmissionReviewResponse
                {
                    Uid = review?.Request?.Uid,
                    Allowed = allowed,
                    Status = new AdmissionResponseStatus
                    {
                        Code = 200,
                        Message = "Admission request allowed"
                    }
                }
            };
        }

        private void AddEvent(AddAppApiRequest request, string eventMessage)
        {
            var model = _mapper.Map<EventModel>(request);
            model.EventMessage = eventMessage;
            _eventService.CreateEvent(model);
        }

        private void CreateGates(AppModel request)
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

        private void DeleteGates(List<GateModel> appGates)
        {
            foreach (var gate in appGates)
            {
                _gateService.RemoveGate(gate);
            }
        }
    }
}
