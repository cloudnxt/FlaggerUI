namespace KubeReview.Models
{
    public class KubeAdmissionReviewRequest
    {
        public string? ApiVersion { get; set; }
        public string? Kind { get; set; }
        public AdmissionRequest? Request { get; set; }
    }
}
