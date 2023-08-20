namespace KubeReview.Models
{
    public class KubeAdmissionReviewResponse
    {
        public string ApiVersion { get; set; }
        public string Kind { get; set; }
        public AdmissionReviewResponse Response { get; set; }
    }

    public class AdmissionReviewResponse
    {
        public string? Uid { get; set; }
        public bool Allowed { get; set; }
        public AdmissionResponseStatus? Status { get; set; }
        public string? PatchType { get; set; }
        public string? Patch { get; set; }
    }

    public class AdmissionResponseStatus
    {
        public int Code { get; set; }
        public string? Message { get; set; }
    }
}
