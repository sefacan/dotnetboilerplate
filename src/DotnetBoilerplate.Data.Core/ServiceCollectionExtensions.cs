using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetBoilerplate.Data.Core
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDataContext(this IServiceCollection services, DataProviderType type, string connectionString)
        {
            services.AddDbContextPool<AppDbContext>(options =>
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
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        }
    }
}