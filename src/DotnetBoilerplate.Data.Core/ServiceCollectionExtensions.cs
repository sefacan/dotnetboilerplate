using DotnetBoilerplate.Data.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDataContext(this IServiceCollection services, DataProviderType type, string connectionString)
        {
            services.AddDbContextPool<DataContextBase>(options =>
            {
                //SqlServer
                if (type == DataProviderType.SqlServer)
                {
                    options.UseSqlServer(connectionString);
                }
                //MySql & MariaDB
                else if (type == DataProviderType.MySql || type == DataProviderType.MariaDB)
                {
                    options.UseMySql(connectionString);
                }
                //PostgreSql
                else if (type == DataProviderType.PostgreSql)
                {
                    options.UseNpgsql(connectionString);
                }
                //InMemory
                else if (type == DataProviderType.InMemory)
                {
                    options.UseInMemoryDatabase("InMemory");
                }

                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });
            services.TryAddScoped(typeof(IRepository<>), typeof(Repository<>));
        }
    }
}