using System;
using System.Collections.Generic;

namespace DotnetBoilerplate.DependencyInjection
{
    public interface IDependencyContext
    {
        IServiceProvider ServiceProvider { get; }

        T GetService<T>();

        IEnumerable<T> GetServices<T>();

        object GetService(Type type);

        IEnumerable<object> GetServices(Type type);

        T GetUnregisteredService<T>(Type type);

        object GetUnregisteredService(Type type);
    }
}