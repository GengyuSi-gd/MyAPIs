using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Response
{
    public class BaseResponse
    {
        public string ResponseId { get; set; }
        public string CorrelationId { get; set; }
        public DateTime ResponseDateTime { get; set; } = DateTime.UtcNow;

        public bool IsSuccess { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public string SubResponseCode { get; set; }
        public string SubResponseMessage { get; set; }
    }
}
