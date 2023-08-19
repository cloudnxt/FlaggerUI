namespace Gates.Shared.Requests
{
    public class AddEventApiRequest : Request
    {
        public string? Phase { get; set; }
        public EventMetadata metadata { get; set; }

    }
    public class EventMetadata
    {
        public string? WebhookState { get; set; }
        public string? EventMessage { get; set; }
    }
}
