using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DotnetBoilerplate.Http
{
    public static class HttpClientExtensions
    {
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

        public static async Task<HttpResponseMessage> PostJsonAsync(this HttpClient client, string requestUri, object data)
        {
            return await client.PostAsync(requestUri, new JsonContent(data));
        }

        public static async Task<HttpResponseMessage> PutJsonAsync(this HttpClient client, string requestUri, object data)
        {
            return await client.PutAsync(requestUri, new JsonContent(data));
        }

        public static async Task<HttpResponseMessage> PatchJsonAsync(this HttpClient client, string requestUri, object data)
        {
            return await client.PatchAsync(requestUri, new JsonContent(data));
        }

        public static async Task<TContent> GetAsync<TContent>(this HttpClient client, string requestUri)
        {
            var responseMessage = await client.GetAsync(requestUri);
            if (responseMessage.IsSuccessStatusCode)
                return await responseMessage.DeserializeAsync<TContent>();

            return default;
        }

        public static async Task<TContent> DeserializeAsync<TContent>(this HttpResponseMessage response)
        {
            var contentStream = await response.Content.ReadAsStreamAsync();
            var content = await JsonSerializer.DeserializeAsync<TContent>(contentStream);

            return content;
        }
    }

    public class JsonContent : StringContent
    {
        public JsonContent(object value)
            : base(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json")
        {
        }

        public JsonContent(object value, Encoding encoding)
            : base(JsonSerializer.Serialize(value), encoding, "application/json")
        {
        }
    }
}