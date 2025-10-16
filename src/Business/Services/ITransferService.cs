using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Request;
using Common.Response;

namespace Business.Services
{
    public interface ITransferService
    {
        Task<BaseResponse> Transfer(BaseRequest request);
    }
}
