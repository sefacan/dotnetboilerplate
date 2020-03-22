using Microsoft.Extensions.DependencyInjection;

namespace DotnetBoilerplate.CorrelationId.DependencyInjection
{
    /// <inheritdoc />
    internal class CorrelationIdBuilder : ICorrelationIdBuilder
    {
        public CorrelationIdBuilder(IServiceCollection services)
        {
            Services = services;
        }

        /// <inheritdoc />
        public IServiceCollection Services { get; }
    }
}