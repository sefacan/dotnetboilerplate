using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBoilerplate.Http
{
    public static class HttpClientExtensions
    {
        public static async Task<TContent> GetAsync<TContent>(this HttpClient client, string requestUri)
        {
            var responseMessage = await client.GetAsync(requestUri);
            if (responseMessage.IsSuccessStatusCode)
                return await responseMessage.ReadJsonAsync<TContent>();

            return default;
        }

        public static async Task<HttpResponseMessage> PostJsonAsync(this HttpClient client, string requestUri, object data)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = CreateContent(data)
            };

            return await client.SendAsync(request);
        }

        public static async Task<HttpResponseMessage> PutJsonAsync(this HttpClient client, string requestUri, object data)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, requestUri)
            {
                Content = CreateContent(data)
            };

            return await client.SendAsync(request);
        }

        public static async Task<HttpResponseMessage> PatchJsonAsync(this HttpClient client, string requestUri, object data)
        {
            var request = new HttpRequestMessage(HttpMethod.Patch, requestUri)
            {
                Content = CreateContent(data)
            };

            return await client.SendAsync(request);
        }

        public static async Task<HttpResponseMessage> DeleteAsync<TContent>(this HttpClient client, string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, url);
            return await client.SendAsync(request);
        }

        public static async Task<HttpResponseMessage> DeleteAsync<TContent>(this HttpClient client, string url, object data)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, url)
            {
                Content = CreateContent(data)
            };

            return await client.SendAsync(request);
        }

        public static void AddHeaderValue(this HttpClient client, string name, string value)
        {
            client.DefaultRequestHeaders.Add(name, value);
        }

        public static void AddJsonContentType(this HttpClient client, bool clearOtherTypes = false)
        {
            if (clearOtherTypes)
                client.DefaultRequestHeaders.Accept.Clear();

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public static void AddBasicAuthentication(this HttpClient client, string userName, string password)
        {
            var basicAuth = Encoding.UTF8.GetBytes(string.Format("{0}:{1}", userName, password));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(basicAuth));
        }

        public static void AddBearerAuthentication(this HttpClient client, string token)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public static async Task<TContent> ReadJsonAsync<TContent>(this HttpResponseMessage httpResponseMessage)
        {
            var content = httpResponseMessage.Content;
            if (content == null)
                return default;

            var contentType = content.Headers.ContentType.MediaType;
            if (!contentType.Contains("application/json"))
                throw new HttpRequestException($"Content type \"{contentType}\" not supported");

            var stream = await httpResponseMessage.Content.ReadAsStreamAsync();
            return Deserialize<TContent>(stream);
        }

        private static HttpContent CreateContent(object data)
        {
            var json = JsonConvert.SerializeObject(data);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        private static T Deserialize<T>(Stream s)
        {
            using (StreamReader reader = new StreamReader(s))
            using (JsonTextReader jsonReader = new JsonTextReader(reader))
            {
                JsonSerializer serializer = new JsonSerializer();
                return serializer.Deserialize<T>(jsonReader);
            }
        }
    }
}