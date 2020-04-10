using System.Collections.Generic;

namespace MUThienSu
{
    public class RequestBuilder
    {
        public string Url { get; private set; }
        public IDictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
        public string HttpMethod { get; set; } = "GET";
        public string ContentType { get; set; }

        public RequestBuilder(string url)
        {
            Url = url;
        }
    }
}