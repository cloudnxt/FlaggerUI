namespace Gates.Shared.Data
{
    public class GateModel : Entity
    {
        public int AppId { get; set; }
        public string Name { get; set; }
        public string Namespace { get; set; }
        public string? Status { get; set; }
        public string WebhookState { get; set; }
        public string? Action { get; set; }
        public bool Waiting { get; set; }
    }
}
