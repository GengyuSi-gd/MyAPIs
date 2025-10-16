namespace Common.Models
{
    public class MultiPartEntity
    {
        public string Name { get; set; }
        public string FileName { get; set; }
        public HttpContent Content { get; set; }
        public string ContentType { get; set; }
    }
}
