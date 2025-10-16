using System.Collections.Generic;
using Common.Helper;
using Common.Response;

namespace Common.Helper
{
    public class MethodExecutionHelper<T> : IMethodExecutionHelper<T>
    {
        public BaseResponse ExecuteMethods(T request, List<MethodItem<T>> methodItems)
        {
            if (methodItems.Count == 0)
            {
                return new BaseResponse { ResponseCode = "Success" };
            }

            BaseResponse currentResponse = null;
            var grouped = false;

            foreach (var method in methodItems)
            {
                if (grouped && !method.FailAsGroup)
                {
                    grouped = false;
                }

                if (!grouped && method.FailAsGroup)
                {
                    grouped = true;
                }

                if (method.IsExecutionRequired)
                    currentResponse = method.Method(request);

                if (currentResponse?.ResponseCode != "Success")
                {
                    if (!grouped) continue;
                    return currentResponse;
                }
            }
            return currentResponse;
        }
    }
}
