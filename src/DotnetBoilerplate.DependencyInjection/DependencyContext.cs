using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DotnetBoilerplate.DependencyInjection
{
    public class DependencyContext : IDependencyContext
    {
        public IServiceProvider ServiceProvider { get; private set; }

        public DependencyContext(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public T GetService<T>()
        {
            return ServiceProvider.GetService<T>();
        }

        public IEnumerable<T> GetServices<T>()
        {
            return ServiceProvider.GetServices<T>();
        }

        public object GetService(Type type)
        {
            return ServiceProvider.GetService(type);
        }

        public IEnumerable<object> GetServices(Type type)
        {
            return ServiceProvider.GetServices(type);
        }

        public T GetUnregisteredService<T>(Type type)
        {
            return (T)GetUnregisteredService(type);
        }

        public object GetUnregisteredService(Type type)
        {
            Exception innerException = null;
            foreach (var constructor in type.GetConstructors())
            {
                try
                {
                    //try to resolve constructor parameters
                    var parameters = constructor.GetParameters().Select(parameter =>
                    {
                        var service = GetService(parameter.ParameterType);
                        if (service == null)
                            throw new InvalidOperationException("Unknown dependency");

                        return service;
                    });

                    //all is ok, so create instance
                    return Activator.CreateInstance(type, parameters.ToArray());
                }
                catch (Exception ex)
                {
                    innerException = ex;
                }
            }

            throw new InvalidOperationException("No constructor was found that had all the dependencies satisfied.", innerException);
        }
    }
}