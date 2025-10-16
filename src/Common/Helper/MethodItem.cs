using Common.Response;

namespace Common.Helper
{
    public class MethodItem<T>
    {
        public MethodItem()
        {
            IsExecutionRequired = true;
        }
        public Func<T, BaseResponse> Method { get; set; }
        public bool IsExecutionRequired { get; set; }
        public bool FailAsGroup { get; set; }
    }
}