using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gates.Shared.Responses
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Data
    {
        public string resultType { get; set; }
        public List<Result> result { get; set; }
    }

    public class Metric
    {
        public string? __name__ { get; set; }
        public string? app_kubernetes_io_instance { get; set; }
        public string? app_kubernetes_io_name { get; set; }
        public string? app_kubernetes_io_version { get; set; }
        public string? instance { get; set; }
        public string? job { get; set; }
        public string? kubernetes_namespace { get; set; }
        public string? kubernetes_pod_name { get; set; }
        public string? name { get; set; }
        public string? @namespace { get; set; }
        public string? pod_template_hash { get; set; }
        public string? status { get; set; }
        
    }

    public class Result
    {
        public Metric metric { get; set; }
        public List<object> value { get; set; }
    }

    public class MetricResponse
    {
        public string status { get; set; }
        public Data data { get; set; }
    }


}
