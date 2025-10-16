namespace Business.Filters
{
    /// <summary>
    /// Custom attribute to flag model properties NOT desired for logging.
    /// The presence of this attribute is checked for in the LogRequest action filter prior to logging.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DoNotLogAttribute : Attribute
    {
    }
}
