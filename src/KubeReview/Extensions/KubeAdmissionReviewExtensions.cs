using KubeReview.Models;

namespace KubeReview.Extensions
{
    public static class KubeAdmissionReviewExtensions
    {

        public static bool IsDeployment(this KubeAdmissionReviewRequest review)
        {
            var kind = review?.Request?.Kind?.Kind;
            if (String.Equals(kind, "deployment", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        public static string? GetReviewKind(this KubeAdmissionReviewRequest review)
        {
            var kind = review?.Request?.Kind?.Kind;
            return kind;
        }
        public static string? GetOperation(this KubeAdmissionReviewRequest review)
        {
            var operation = review?.Request?.Operation;
            return operation;
        }

        public static Dictionary<string,string>? GetAnnotations(this KubeAdmissionReviewRequest review)
        {
            return review?.Request?.Object?.Metadata?.Annotations;
        }
        public static string? GetResourceNamespace(this KubeAdmissionReviewRequest review)
        {
            return review?.Request?.Object?.Metadata?.Namespace;
        }
        public static string? GetResourceName(this KubeAdmissionReviewRequest review)
        {
            return review?.Request?.Object?.Metadata?.Name;
        }

        public static List<Container>? GetContainers(this KubeAdmissionReviewRequest review)
        {
            return review?.Request?.Object?.spec?.template?.spec?.containers;
        }

        public static KubeAdmissionReviewResponse SendSuccessResponse(string uid)
        {
            // Create the AdmissionResponse object
            return new KubeAdmissionReviewResponse
            {
                ApiVersion = "admission.k8s.io/v1",
                Kind = "AdmissionReview",
                Response = new AdmissionReviewResponse
                {
                    Uid = uid,
                    Allowed = true,
                    Status = new AdmissionResponseStatus
                    {
                        Code = 200,
                        Message = "Admission request allowed"
                    }
                }
            };
        }
        public static KubeAdmissionReviewResponse SendFaliureResponse(string uid)
        {
            // Create the AdmissionResponse object
            return new KubeAdmissionReviewResponse
            {
                ApiVersion = "admission.k8s.io/v1",
                Kind = "AdmissionReview",
                Response = new AdmissionReviewResponse
                {
                    Uid = uid,
                    Allowed = false,
                    Status = new AdmissionResponseStatus
                    {
                        Code = 400,
                        Message = "Admission request not allowed"
                    }
                }
            };
        }

    }
}
