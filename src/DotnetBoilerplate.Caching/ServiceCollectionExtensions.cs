using DotnetBoilerplate.Caching;
using EasyCaching.Core.Configurations;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Net;

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
            var configurationOptions = ConfigurationOptions.Parse(connectionString);

            services.AddEasyCaching(options =>
            {
                //use memory cache that named default
                options.UseRedis(config =>
                {
                    var dnsEndpoints = configurationOptions.EndPoints.Select(endpoint => endpoint as DnsEndPoint).ToList();
                    for (int i = 0; i < dnsEndpoints.Count; i++)
                        config.DBConfig.Endpoints.Add(new ServerEndPoint(dnsEndpoints[i].Host, dnsEndpoints[i].Port));

                    config.DBConfig.ConnectionTimeout = 5000;
                    config.DBConfig.AllowAdmin = configurationOptions.AllowAdmin;
                    config.DBConfig.IsSsl = configurationOptions.Ssl;

                    //set ssl host
                    if (configurationOptions.Ssl)
                        config.DBConfig.SslHost = configurationOptions.SslHost;

                    //set password
                    if (!string.IsNullOrEmpty(configurationOptions.Password))
                        config.DBConfig.Password = configurationOptions.Password;
                });
            });
            services.AddScoped<ICacheManager, DefaultCacheManager>();
        }
    }
}