using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace DotnetBoilerplate.CorrelationId
{
    public static class HttpClientBuilderExtensions
    {
        public static IHttpClientBuilder AddCorrelationIdForwarding(this IHttpClientBuilder builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.TryAddTransient<CorrelationIdMessageHandler>();
            builder.AddHttpMessageHandler<CorrelationIdMessageHandler>();

            return builder;
        }
    }
}