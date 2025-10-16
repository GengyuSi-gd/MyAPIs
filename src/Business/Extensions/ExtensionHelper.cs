using System.Reflection;
using Common.Response;

namespace Business.Extensions
{
    public static class ExtensionHelper
    {
        // A property is defined as a list if it implements IEnumerable and isn't a string.
        public static bool IsList(this PropertyInfo p)
        {
            return p.PropertyType != typeof(String) && typeof(IEnumerable<object>).IsAssignableFrom(p.PropertyType);
        }

        // Check if a type is a primitive or a string.
        public static bool IsPrimitiveOrString(this Type t)
        {
            return t == typeof(String) || t.IsValueType;
        }

        // Check if a response is a success or not.
        public static bool Success(this BaseResponse response)
        {
            return response.ResponseCode.Equals("0");
        }

        public static string ToISO8601DateString(this DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }
    }
}
