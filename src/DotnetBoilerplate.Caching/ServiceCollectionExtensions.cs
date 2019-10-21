using DotnetBoilerplate.Caching;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddEasyMemoryCaching(this IServiceCollection services)
        {
            services.AddEasyCaching(options =>
            {
                //use memory cache that named default
                options.UseInMemory();
            });
            services.AddScoped<ICacheManager, DefaultCacheManager>();
        }

        public static void AddEasyRedisCaching(this IServiceCollection services, string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString), "Redis connection string can not empty.");

            //Example connection string: localhost:6379;PASSWORD;allowAdmin=true
            string host = connectionString.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[0];
            if (string.IsNullOrWhiteSpace(host))
                throw new ArgumentNullException(nameof(host), "Redis connection host info can not empty.");

            string password = connectionString.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)[1];
            if (!int.TryParse(connectionString.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[1], out int port))
                throw new ArgumentException(nameof(host), "Invalid redis connection port.");

            string allowAdminStr = connectionString.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)[2];
            bool.TryParse(allowAdminStr.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)[1], out bool allowAdmin);
            if (string.IsNullOrWhiteSpace(allowAdminStr))
                allowAdmin = true;

            string useSslStr = connectionString.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)[3];
            bool.TryParse(useSslStr.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)[1], out bool useSsl);

            services.AddEasyCaching(options =>
            {
                //use memory cache that named default
                options.UseRedis(config =>
                {
                    config.DBConfig.Endpoints.Add(new EasyCaching.Core.Configurations.ServerEndPoint(host, port));
                    config.DBConfig.ConnectionTimeout = 5000;
                    config.DBConfig.AllowAdmin = allowAdmin;
                    config.DBConfig.IsSsl = useSsl;

                    //set ssl host
                    if (useSsl)
                        config.DBConfig.SslHost = host;

                    //set password
                    if (!string.IsNullOrEmpty(password))
                        config.DBConfig.Password = password;
                });
            });
            services.AddScoped<ICacheManager, DefaultCacheManager>();
        }
    }
}