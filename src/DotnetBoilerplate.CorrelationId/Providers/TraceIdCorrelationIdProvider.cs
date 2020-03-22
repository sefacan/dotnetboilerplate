using DotnetBoilerplate.CorrelationId.Abstractions;
using Microsoft.AspNetCore.Http;

namespace DotnetBoilerplate.CorrelationId.Providers
{
    /// <summary>
    /// Sets the correlation ID to match the TraceIdentifier set on the <see cref="HttpContext"/>.
    /// </summary>
    public class TraceIdCorrelationIdProvider : ICorrelationIdProvider
    {
        /// <inheritdoc />
        public string GenerateCorrelationId(HttpContext ctx) => ctx.TraceIdentifier;
    }
}