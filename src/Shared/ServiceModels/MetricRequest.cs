using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gates.Shared.ServiceModels
{
    public class MetricRequest
    {
        public string appname { get; set; }

        public string ClusterName { get; set; }

        public string Namespace { get; set; }
    }
}
