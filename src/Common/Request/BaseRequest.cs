using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Request
{
    public class BaseRequest
    {
        public string RequestId { get; set; }
        public string CorrelationId { get; set; }
        public string Locale { get; set; }
        public DateTime RequestDateTime { get; set; } = DateTime.UtcNow;
    }
}
