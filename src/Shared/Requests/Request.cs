namespace Gates.Shared.Requests
{
    public abstract class Request
    {
        public string Name { get; set; }
        public string? Phase { get; set; }
        public string Namespace { get; set; }
    }
}
