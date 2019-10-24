using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using System;

namespace DotnetBoilerplate.Serilog
{
    public static class LoggerConfigurationExtensions
    {
        //public static LoggerConfiguration MSSqlServer(this LoggerSinkConfiguration loggerConfiguration,
        //       string connectionString,
        //       string tableName = "Logs",
        //       string schemaName = "dbo",
        //       bool autoCreateSqlTable = false,
        //       LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum)
        //{
        //    if (string.IsNullOrWhiteSpace(connectionString))
        //        throw new ArgumentNullException(nameof(connectionString));

        //    return loggerConfiguration.Sink(new MSSqlServerSink(connectionString, tableName, schemaName, autoCreateSqlTable), restrictedToMinimumLevel);
        //}

        //public static LoggerConfiguration MariaDB(this LoggerSinkConfiguration loggerConfiguration,
        //       string connectionString,
        //       string tableName = "Logs",
        //       string schemaName = "dbo",
        //       bool autoCreateSqlTable = false,
        //       LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum)
        //{
        //    if (string.IsNullOrWhiteSpace(connectionString))
        //        throw new ArgumentNullException(nameof(connectionString));

        //    return loggerConfiguration.Sink(new MariaDBSink(connectionString, tableName, schemaName, autoCreateSqlTable), restrictedToMinimumLevel);
        //}

        //public static LoggerConfiguration Mysql(this LoggerSinkConfiguration loggerConfiguration,
        //       string connectionString,
        //       string tableName = "Logs",
        //       string schemaName = "dbo",
        //       bool autoCreateSqlTable = false,
        //       LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum)
        //{
        //    if (string.IsNullOrWhiteSpace(connectionString))
        //        throw new ArgumentNullException(nameof(connectionString));

        //    return loggerConfiguration.Sink(new MysqlSink(connectionString, tableName, schemaName, autoCreateSqlTable), restrictedToMinimumLevel);
        //}

        //public static LoggerConfiguration PostgreSql(this LoggerSinkConfiguration loggerConfiguration,
        //       string connectionString,
        //       string tableName = "Logs",
        //       string schemaName = "dbo",
        //       bool autoCreateSqlTable = false,
        //       LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum)
        //{
        //    if (string.IsNullOrWhiteSpace(connectionString))
        //        throw new ArgumentNullException(nameof(connectionString));

        //    return loggerConfiguration.Sink(new PostgreSqlSink(connectionString, tableName, schemaName, autoCreateSqlTable), restrictedToMinimumLevel);
        //}
    }
}