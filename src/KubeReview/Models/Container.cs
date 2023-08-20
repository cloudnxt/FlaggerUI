using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KubeReview.Models
{
    public class Container
    {
        public string name { get; set; }
        public string image { get; set; }
        public List<Port>? ports { get; set; }
        public Probe? livenessProbe { get; set; }
        public Probe? readinessProbe { get; set; }
        public string? imagePullPolicy { get; set; }

    }
    public class Exec
    {
        public List<string> command { get; set; }
    }
    public class Probe
    {
        public Exec exec { get; set; }
        public int initialDelaySeconds { get; set; }
        public int timeoutSeconds { get; set; }
    }

    public class Port
    {
        public string? name { get; set; }
        public int containerPort { get; set; }
        public string? protocol { get; set; }
    }
}
