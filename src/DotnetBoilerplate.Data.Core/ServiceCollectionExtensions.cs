using DotnetBoilerplate.Data.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDataContextPool<TContext>(this IServiceCollection services, DataProviderType type, string connectionString, int poolSize = 128) where TContext : DbContext
        {
            services.AddDbContextPool<TContext>(options =>
            {
                switch (type)
                {
                    //InMemory
                    case DataProviderType.InMemory:
                    options.UseInMemoryDatabase("InMemory");
                    break;
                    //SqlServer
                    case DataProviderType.SqlServer:
                    options.UseSqlServer(connectionString);
                    break;
                    //MySql & MariaDB
                    case DataProviderType.MySql:
                    case DataProviderType.MariaDB:
                    options.UseMySql(connectionString);
                    break;
                    //PostgreSql
                    case DataProviderType.PostgreSql:
                    options.UseNpgsql(connectionString);
                    break;
                }

                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            }, poolSize);

            //register repository
            services.TryAddScoped(typeof(IRepository<>), typeof(Repository<>));
        }

        public static void AddDataContext<TContext>(this IServiceCollection services, DataProviderType type, string connectionString) where TContext : DbContext
        {
            services.AddDbContext<TContext>(options =>
            {
                switch (type)
                {
                    //InMemory
                    case DataProviderType.InMemory:
                    options.UseInMemoryDatabase("InMemory");
                    break;
                    //SqlServer
                    case DataProviderType.SqlServer:
                    options.UseSqlServer(connectionString);
                    break;
                    //MySql & MariaDB
                    case DataProviderType.MySql:
                    case DataProviderType.MariaDB:
                    options.UseMySql(connectionString);
                    break;
                    //PostgreSql
                    case DataProviderType.PostgreSql:
                    options.UseNpgsql(connectionString);
                    break;
                }

                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            //register repository
            services.TryAddScoped(typeof(IRepository<>), typeof(Repository<>));
        }
    }
}