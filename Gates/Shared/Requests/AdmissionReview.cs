namespace Gates.Shared.Requests
{

    public class AdmissionReview
    {
        public string? ApiVersion { get; set; }
        public string? Kind { get; set; }
        public AdmissionRequest? Request { get; set; }
    }

    public class KindInfo
    {
        public string Group { get; set; }
        public string Version { get; set; }
        public string Kind { get; set; }
    }
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
        public Spec spec { get; set; }
    }
    public class Spec
    {
        public int replicas { get; set; }
        public Template template { get; set; } // This should be a Dictionary since it contains a nested object

    }

    public class Template
    {
        public PodSpec? spec { get; set; }
    }

    public class PodSpec
    {
        public List<Container> containers { get; set; }
    }

    public class ObjectMetadata
    {
        public string? Name { get; set; }
        public string? Namespace { get; set; }
        public Dictionary<string, string>? Labels { get; set; }
        public Dictionary<string, string>? Annotations { get; set; }
    }

    public class Container
    {
        public string name { get; set; }
        public string image { get; set; }

        public List<Port> ports { get; set; }
    }
    public class Port
    {
        public string name { get; set; }
        public int containerPort { get; set; }
        public string protocol { get; set; }
    }


}





//{
//  "apiVersion": "admission.k8s.io/v1",
//  "kind": "AdmissionReview",
//  "request": {
//    "uid": "12345678-1234-5678-9876-1234567890AB",
//    "kind": {
//      "group": "",
//      "version": "v1",
//      "kind": "Pod"
//    },
//    "resource": {
//      "group": "",
//      "version": "v1",
//      "resource": "pods"
//    },
//    "namespace": "mynamespace",
//    "operation": "CREATE",
//    "userInfo": {
//      "username": "john.doe",
//      "groups": ["developers"]
//    },
//    "object": {
//      "metadata": {
//        "name": "mypod",
//        "namespace": "mynamespace",
//        "labels": {
//                    "app": "myapp"
//        },
//        "annotations": {
//                    "created-by": "john.doe"
//        }
//            },
//      "spec": {
//                "containers": [
//                  {
//                    "name": "mycontainer",
//            "image": "nginx:latest",
//            "ports": [
//              {
//                        "containerPort": 80
//              }
//            ]
//          }
//        ]
//      }
//        }
//    }
//}