using System;
using DotnetBoilerplate.CorrelationId.Abstractions;
using Microsoft.AspNetCore.Http;

namespace DotnetBoilerplate.CorrelationId.Providers
{
    /// <summary>
    /// Generates a correlation ID using a new GUID.
    /// </summary>
    public class GuidCorrelationIdProvider : ICorrelationIdProvider
    {
        /// <inheritdoc />
        public string GenerateCorrelationId(HttpContext ctx) => Guid.NewGuid().ToString();
    }
}