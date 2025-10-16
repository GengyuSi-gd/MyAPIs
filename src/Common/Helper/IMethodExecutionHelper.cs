using System.Collections.Generic;
using Common.Response;

namespace Common.Helper
{
    public interface IMethodExecutionHelper<T>
    {
        BaseResponse ExecuteMethods(T request, List<MethodItem<T>> methodItems);
    }
}
