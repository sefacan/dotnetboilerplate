using System;
using System.Net.Http;
using System.Text;

namespace DotnetBoilerplate.Http
{
    public static class HttpContentExtentions
    {
        public static HttpContent AddHeaderValue(this HttpContent content, string name, string value)
        {
            content.Headers.Add(name, value);
            return content;
        }

        public static HttpContent AddBasicAuthentication(this HttpContent content, string userName, string password)
        {
            var basicAuth = Encoding.UTF8.GetBytes($"{userName}:{password}");
            content.Headers.Add("Authentication", $"Basic {Convert.ToBase64String(basicAuth)}");

            return content;
        }

        public static HttpContent AddBearerAuthentication(this HttpContent content, string token)
        {
            content.Headers.Add("Authentication", $"Bearer {token}");
            return content;
        }
    }
}
