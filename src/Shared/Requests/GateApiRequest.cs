namespace Gates.Shared.Requests
{
    public class GateApiRequest : Request
    {

        public string? Status { get; set; }
        public GateMetadata Metadata { get; set; }

    }
    public class GateMetadata
    {
        public string WebhookState { get; set; }
        public string? Action { get; set; }

    }
}
