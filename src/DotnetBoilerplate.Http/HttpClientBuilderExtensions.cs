using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DotnetBoilerplate.Http
{
    public static class HttpClientBuilderExtensions
    {
        public static IHttpClientBuilder AddPolicyHandlers(this IHttpClientBuilder httpClientBuilder, string policySectionName, IConfiguration configuration)
        {
            var serviceProvider = httpClientBuilder.Services.BuildServiceProvider();
            var logger = serviceProvider.GetService<ILogger>();

            var policyConfig = new PolicyConfig();
            configuration.Bind(policySectionName, policyConfig);

            var circuitBreakerPolicyConfig = (ICircuitBreakerPolicyConfig)policyConfig;
            var retryPolicyConfig = (IRetryPolicyConfig)policyConfig;

            return httpClientBuilder.AddRetryPolicyHandler(logger, retryPolicyConfig)
                                    .AddCircuitBreakerHandler(logger, circuitBreakerPolicyConfig);
        }

        public static IHttpClientBuilder AddRetryPolicyHandler(this IHttpClientBuilder httpClientBuilder, ILogger logger, IRetryPolicyConfig retryPolicyConfig)
        {
            return httpClientBuilder.AddPolicyHandler(HttpRetryPolicies.GetHttpRetryPolicy(logger, retryPolicyConfig));
        }

        public static IHttpClientBuilder AddCircuitBreakerHandler(this IHttpClientBuilder httpClientBuilder, ILogger logger, ICircuitBreakerPolicyConfig circuitBreakerPolicyConfig)
        {
            return httpClientBuilder.AddPolicyHandler(HttpCircuitBreakerPolicies.GetHttpCircuitBreakerPolicy(logger, circuitBreakerPolicyConfig));
        }
    }
}