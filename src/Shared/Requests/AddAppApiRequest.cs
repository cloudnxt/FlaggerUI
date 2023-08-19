namespace Gates.Shared.Requests
{
    public class AddAppApiRequest : Request
    {
        public string? Url { get; set; }
        public string? Image { get; set; }
        public int? Replicas { get; set; }
        public string? ContainerPorts { get; set; }
    }
}
