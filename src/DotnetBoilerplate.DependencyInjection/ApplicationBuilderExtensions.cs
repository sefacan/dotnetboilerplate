using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetBoilerplate.DependencyInjection
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseAdvancedDependencyInjection(this IApplicationBuilder app)
        {
            var dependencyContext = app.ApplicationServices.GetService<IDependencyContext>();
            ServiceLocator.Initialize(dependencyContext);

            return app;
        }
    }
}