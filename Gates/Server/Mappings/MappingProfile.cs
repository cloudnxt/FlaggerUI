using AutoMapper;
using Gates.Shared.Data;
using Gates.Shared.Requests;
using Gates.Shared.Responses;

namespace Gates.Server.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AddAppApiRequest, AppModel>();
            CreateMap<AddAppApiRequest, EventModel>();

            CreateMap<AddEventApiRequest,EventModel>()
                .ForMember(pts => pts.WebhookState, opt => opt.MapFrom(ps => ps.metadata.WebhookState))
                .ForMember(pts => pts.EventMessage, opt => opt.MapFrom(ps => ps.metadata.EventMessage));

            CreateMap<GateApiRequest, GateModel>()
             .ForMember(pts => pts.WebhookState, opt => opt.MapFrom(ps => ps.Metadata.WebhookState))
             .ForMember(pts => pts.Action, opt => opt.MapFrom(ps => ps.Metadata.Action));

            CreateMap<RunLoadTestApiRequest, LoadTestModel>()
                .ForMember(pts => pts.WebhookState, opt => opt.MapFrom(ps => ps.Metadata.WebhookState))
                .ForMember(pts => pts.Url, opt => opt.MapFrom(ps => ps.Metadata.Url))
                .ForMember(pts => pts.Method, opt => opt.MapFrom(ps => ps.Metadata.Method))
                .ForMember(pts => pts.Payload, opt => opt.MapFrom(ps => ps.Metadata.Payload))
                .ForMember(pts => pts.NoOfRequests, opt => opt.MapFrom(ps => ps.Metadata.NoOfRequests));
        }
    }
}
