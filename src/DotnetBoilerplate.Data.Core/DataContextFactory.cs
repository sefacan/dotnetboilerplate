using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;

namespace DotnetBoilerplate.Data.Core
{
    public class DataContextFactory : IDesignTimeDbContextFactory<DataContextBase>
    {
        public DataContextBase CreateDbContext(string[] args)
        {
            var builder = new HostBuilder();
            builder.UseContentRoot(Directory.GetCurrentDirectory());
            builder.ConfigureAppConfiguration((hostingContext, config) =>
            {
                var env = hostingContext.HostingEnvironment;

                config.AddJsonFile("appsettings.json")
                      .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
                config.AddEnvironmentVariables();
            });

            var host = builder.Build();
            var config = (IConfiguration)host.Services.GetService(typeof(IConfiguration));
            
            var connectionString = config.GetConnectionString("Default");
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Could not find a connection string named 'Default'.");

            var optionsBuilder = new DbContextOptionsBuilder<DataContextBase>();
            optionsBuilder.UseNpgsql(connectionString, sqlOptions =>
            {
                sqlOptions.MigrationsHistoryTable("_MigrationsHistory");
            });

            return new DataContextBase(optionsBuilder.Options);
        }
    }
}