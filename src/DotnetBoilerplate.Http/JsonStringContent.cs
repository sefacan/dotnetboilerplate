using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace DotnetBoilerplate.Http
{
    public class JsonStringContent : StringContent
    {
        public JsonStringContent(object data)
            : this(data, Encoding.UTF8, "application/json")
        {
        }

        public JsonStringContent(object data, Encoding encoding)
            : this(data, encoding, "application/json")
        {
        }

        public JsonStringContent(object data, Encoding encoding, string contentType)
            : base(JsonConvert.SerializeObject(data), encoding, contentType)
        {
        }
    }
}
