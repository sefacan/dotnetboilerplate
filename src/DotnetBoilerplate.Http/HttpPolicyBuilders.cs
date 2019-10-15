using Polly;
using Polly.Extensions.Http;
using System.Net.Http;

namespace DotnetBoilerplate.Http
{
    public static class HttpPolicyBuilders
    {
        public static PolicyBuilder<HttpResponseMessage> GetBaseBuilder()
        {
            return HttpPolicyExtensions.HandleTransientHttpError();
        }
    }
}