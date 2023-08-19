using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gates.Shared.Requests
{
    public class RunLoadTestApiRequest : Request
    {

        public LoadTestMetadata Metadata { get; set; }

    }
    public class LoadTestMetadata
    {
        public string WebhookState { get; set; }
        public string Url { get; set; }
        public string Method { get; set; }
        public string Payload { get; set; }
        public int NoOfRequests { get; set; }
    }

}
