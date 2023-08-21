namespace KubeReview.Models
{
    public class AdmissionRequest
    {
        public string? Uid { get; set; }
        public KindInfo? Kind { get; set; }
        public ResourceInfo? Resource { get; set; }
        public string? Name { get; set; }
        public string? Namespace { get; set; }
        public string? Operation { get; set; }
        public KubernetesObject? Object { get; set; }
        public KubernetesObject? OldObject { get; set; }
        public KindInfo? requestKind { get; set; }
        public ResourceInfo? requestResource { get; set; }
        public UserInfo? userInfo { get; set; }
        public bool dryRun { get; set; }
        public Options? options { get; set; }
    }

    public class ResourceInfo
    {
        public string? Group { get; set; }
        public string? Version { get; set; }
        public string? Resource { get; set; }
    }

    public class KubernetesObject
    {
        public ObjectMetadata? Metadata { get; set; }
        public Spec? spec { get; set; }
    }
    public class Spec
    {
        public int replicas { get; set; }
        public Template? template { get; set; } // This should be a Dictionary since it contains a nested object

        public TargetRef? targetRef { get; set; }

        public TargetAnalysis? analysis { get; set; }

    }

    public class TargetAnalysis
    {
        public string? interval { get; set; }
        public int? threshold { get; set; }
        public int? iterations { get; set; }
        public int? maxWeight { get; set; }
        public int? stepWeight { get; set; }

        public List<TargetWebhook>? webhooks { get; set; }

    }

    public class TargetWebhook
    {
        public string? name { get; set; }
        public string? type { get; set; }
        public string? url { get; set; }
    }

    public class Template
    {
        public PodSpec? spec { get; set; }
    }

    public class PodSpec
    {
        public List<Container>? containers { get; set; }
    }

    public class ObjectMetadata
    {
        public string? Name { get; set; }
        public string? Namespace { get; set; }
        public Dictionary<string, string>? Labels { get; set; }
        public Dictionary<string, string>? Annotations { get; set; }
    }

    

    public class UserInfo
    {
        public string? username { get; set; }
        public string? uid { get; set; }
    }

    public class Options
    {
        public string? kind { get; set; }
        public string? apiVersion { get; set; }
    }

    public class TargetRef
    {
        public string? kind { get; set; }
        public string? apiVersion { get; set; }
        public string? name { get; set; }
    }
}
