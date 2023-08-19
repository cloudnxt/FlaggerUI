namespace Gates.Shared.Data
{

    public class LoadTestModel : Entity
    {
        public string Name { get; set; }
        public string Phase { get; set; }
        public string Namespace { get; set; }
        public string WebhookState { get; set; }
        public string Url { get; set; }
        public string Method { get; set; }
        public string Payload { get; set; }
        public int NoOfRequests { get; set; }
        public bool Completed { get; set; } = false;
    }
}