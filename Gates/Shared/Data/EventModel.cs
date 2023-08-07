namespace Gates.Shared.Data
{

    public class EventModel : Entity
    {
        public string Name { get; set; }
        public string Namespace { get; set; }
        public string? Status { get; set; }
        public string? Phase { get; set; }
        public string? WebhookState { get; set; }
        public string? EventMessage { get; set; }
    }

}