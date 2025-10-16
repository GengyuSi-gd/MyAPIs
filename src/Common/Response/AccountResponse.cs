using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Response
{
    public class AccountResponse: BaseResponse
    {
        public string AccountId { get; set; }
        public string AccountName { get; set; }
        public string Status { get; set; }
    }
}
