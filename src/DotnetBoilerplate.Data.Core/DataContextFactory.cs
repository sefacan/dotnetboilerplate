//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Design;
//using Microsoft.Extensions.Configuration;
//using System;
//using System.IO;

//namespace DotnetBoilerplate.Data.Core
//{
//    public class DataContextFactory : IDesignTimeDbContextFactory<AppDbContext>
//    {
//        public AppDbContext CreateDbContext(string[] args)
//        {
//            string environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
//            var builder = new ConfigurationBuilder()
//                .SetBasePath(Directory.GetCurrentDirectory())
//                .AddJsonFile("appsettings.json")
//                .AddJsonFile($"appsettings.{environmentName}.json", true)
//                .AddEnvironmentVariables();

//            var config = builder.Build();
//            var connectionString = config.GetConnectionString("Default");
//            if (string.IsNullOrWhiteSpace(connectionString))
//                throw new InvalidOperationException("Could not find a connection string named 'Default'.");

//            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
//            optionsBuilder.UseNpgsql(connectionString, sqlOptions => {
//                sqlOptions.MigrationsHistoryTable("_MigrationsHistory");
//            });

//            return new AppDbContext(optionsBuilder.Options);
//        }
//    }
//}