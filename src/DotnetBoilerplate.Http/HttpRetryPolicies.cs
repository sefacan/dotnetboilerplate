using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using System;
using System.Net.Http;

namespace DotnetBoilerplate.Http
{
    public class HttpRetryPolicies
    {
        public static AsyncRetryPolicy<HttpResponseMessage> GetHttpRetryPolicy(ILogger logger, IRetryPolicyConfig retryPolicyConfig)
        {
            return HttpPolicyExtensions.HandleTransientHttpError()
                                          .WaitAndRetryAsync(retryPolicyConfig.RetryCount,
                                                             ComputeDuration,
                                                             (result, timeSpan, retryCount, context) =>
                                                             {
                                                                 OnHttpRetry(result, timeSpan, retryCount, context, logger);
                                                             });
        }

        private static void OnHttpRetry(DelegateResult<HttpResponseMessage> result, TimeSpan timeSpan, int retryCount, Context context, ILogger logger)
        {
            if (result.Result != null)
                logger.LogWarning("Request failed with {StatusCode}. Waiting {timeSpan} before next retry. Retry attempt {retryCount}", result.Result.StatusCode, timeSpan, retryCount);
            else
                logger.LogWarning("Request failed because network failure. Waiting {timeSpan} before next retry. Retry attempt {retryCount}", timeSpan, retryCount);
        }

        private static TimeSpan ComputeDuration(int input)
        {
            return TimeSpan.FromSeconds(Math.Pow(2, input)) + TimeSpan.FromMilliseconds(new Random().Next(0, 100));
        }
    }
}