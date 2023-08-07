namespace Gates.Shared.Data
{
    public class LoadTestLogModel : Entity
    {
        public int LoadTestId { get; set; }
        public string Url { get; set; }

        public string HttpStatus { get; set; }

        public string? Response { get; set; }


    }
}
