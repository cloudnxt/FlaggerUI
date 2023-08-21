namespace Gates.Shared.Data
{
    public class CanaryModel : Entity
    {
        public int AppId { get; set; }
        public string Name { get; set; }
        public string Namespace { get; set; }
        public string? Status { get; set; }
        public string? Webhooks { get; set; }

        public string? interval { get; set; }
        public int? threshold { get; set; }
        public int? iterations { get; set; }
        public int? maxWeight { get; set; }
        public int? stepWeight { get; set; }

    }
}
