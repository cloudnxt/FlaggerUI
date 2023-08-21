using AutoMapper;
using Gates.Server.Service;
using Gates.Shared.Data;
using Gates.Shared.Extension;
using Gates.Shared.Requests;
using KubeReview.Extensions;
using KubeReview.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Gates.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdmissionWebhookController : ControllerBase
    {
        private readonly IAppService _appService;
        private readonly ICanaryService _canaryService;
        private readonly IMapper _mapper;
        private readonly IGateService _gateService;
        private readonly IEventService _eventService;
        private readonly ILogger<AppController> _logger;
        private readonly string requiredAnnotation = "enable-canary-gates";
        public AdmissionWebhookController(IAppService appService, ICanaryService canaryService, IMapper mapper, IGateService gateService, IEventService eventService, ILogger<AppController> logger)
        {
            _appService = appService;
            _canaryService = canaryService;
            _mapper = mapper;
            _gateService = gateService;
            _eventService = eventService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<KubeAdmissionReviewResponse> Post([FromBody] KubeAdmissionReviewRequest review)
        {
            _logger.LogInformation(JsonSerializer.Serialize(review));
            if (review.Request.dryRun)
            {
                return KubeAdmissionReviewExtensions.SendSuccessResponse(review.Request.Uid);
            }
            if (review.IsDeployment())
            {
                _logger.LogInformation($"Found Deployment");
                return await HandleDeploymentWebhook(review);
            }

            else if (String.Equals(review.GetReviewKind(), "canary", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation($"Found Canary");
                return await HandleCanaryWebhook(review);
            }
            else
            {
                return KubeAdmissionReviewExtensions.SendSuccessResponse(review.Request.Uid);
            }
        }


        private async Task<KubeAdmissionReviewResponse> HandleCanaryWebhook(KubeAdmissionReviewRequest review)
        {
            try
            {
                var operation = review.GetOperation();
                var space = review?.Request.Object.Metadata.Namespace;
                var app = review.Request?.Object?.spec?.targetRef?.name;

                switch (operation)
                {
                    case "CREATE":
                        _logger.LogInformation("Request Operation is CREATE");

                        var appExists = await _appService.GetAppNameAndSpace(app, space);
                        if (appExists != null)
                        {
                            CanaryModel request = new CanaryModel()
                            {
                                AppId = appExists.Id,
                                Name = review.Request.Object.spec?.targetRef?.name,
                                Namespace = review?.Request?.Object?.Metadata?.Namespace,
                                interval = review?.Request?.Object?.spec?.analysis?.interval,
                                threshold = review?.Request?.Object?.spec?.analysis?.threshold,
                                Webhooks = JsonSerializer.Serialize(review?.Request.Object?.spec?.analysis?.webhooks)
                            };

                            await _canaryService.CreateCanary(request);

                        }
                        break;

                    case "DELETE":
                        _logger.LogInformation("in DELETE");

                        var existingApp = await _appService.GetAppNameAndSpace(app, space);
                        if (existingApp != null)
                        {
                            await _canaryService.DeleteCanary(existingApp.Id);
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
            return KubeAdmissionReviewExtensions.SendSuccessResponse(review.Request.Uid);
        }
        private async Task<KubeAdmissionReviewResponse> HandleDeploymentWebhook(KubeAdmissionReviewRequest review)
        {
            try
            {
                var operation = review.GetOperation();

                switch (operation)
                {
                    case "CREATE":
                        _logger.LogInformation("Request Operation is CREATE");

                        var annotationExists = review.GetAnnotations()?.FirstOrDefault(r => r.Key == requiredAnnotation).Value;
                        if (annotationExists == "true")
                        {
                            var space = review?.GetResourceNamespace();
                            var app = review.GetResourceName();

                            if (!app.IsFlaggerResource())
                            {
                                var appExists = await _appService.GetAppNameAndSpace(app, space);
                                if (appExists == null)
                                {
                                    AddAppApiRequest request = PrepareAddAppRequest(review);
                                    var appModel = _mapper.Map<AppModel>(request);
                                    await _appService.CreateApp(appModel);

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
                            if (!app.IsFlaggerResource())
                            {
                                var exists = await _appService.GetAppNameAndSpace(app, space);
                                if (exists != null)
                                {
                                    await _appService.DeleteApp(exists.Id);
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
            return KubeAdmissionReviewExtensions.SendSuccessResponse(review.Request.Uid);
        }

        private static AddAppApiRequest PrepareAddAppRequest(KubeAdmissionReviewRequest review)
        {
            var space = review?.GetResourceNamespace();
            var app = review.GetResourceName();
            var containers = review?.GetContainers()?.Where(c => c.name != "kuma-sidecar");
            var images = string.Join(", ", containers.Select(c => c.image));
            var ports = containers?.SelectMany(c => c.ports);
            var port_string = string.Join(", ", ports.Select(p => $"{p.containerPort}/{p.protocol}"));
            var replicas = review?.Request?.Object?.spec?.replicas;

            AddAppApiRequest request = new AddAppApiRequest();
            request.Name = app;
            request.Namespace = space;
            request.Phase = "Registering";
            request.Url = $"{app}.{space}.svc";
            request.Image = images;
            request.Replicas = replicas;
            request.ContainerPorts = port_string;
            return request;
        }
    }
}
