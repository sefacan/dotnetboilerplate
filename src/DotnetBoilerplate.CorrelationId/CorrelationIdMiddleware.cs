using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetBoilerplate.CorrelationId
{
    /// <summary>
    /// Middleware which attempts to reads / creates a Correlation ID that can then be used in logs and 
    /// passed to upstream requests.
    /// </summary>
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        private const string CorrelationIdHeaderName = "X-Correlation-ID";

        /// <summary>
        /// Creates a new instance of the CorrelationIdMiddleware.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <param name="logger">The <see cref="ILogger"/> instance to log to.</param>
        /// <param name="options">The configuration options.</param>
        /// <param name="correlationIdProvider"></param>
        public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Processes a request to synchronise TraceIdentifier and Correlation ID headers. Also creates a 
        /// <see cref="CorrelationContext"/> for the current request and disposes of it when the request is completing.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/> for the current request.</param>
        /// <param name="correlationContextFactory">The <see cref="ICorrelationContextFactory"/> which can create a <see cref="CorrelationContext"/>.</param>
        public async Task Invoke(HttpContext context, ICorrelationContextFactory correlationContextFactory)
        {
            Log.CorrelationIdProcessingBegin(_logger);

            var hasCorrelationIdHeader = context.Request.Headers.TryGetValue(CorrelationIdHeaderName, out var cid) &&
                                           !StringValues.IsNullOrEmpty(cid);

            var correlationId = hasCorrelationIdHeader ? cid.FirstOrDefault() : null;

            if (hasCorrelationIdHeader)
            {
                Log.FoundCorrelationIdHeader(_logger, correlationId);
            }
            else
            {
                Log.MissingCorrelationIdHeader(_logger);
            }

            if (RequiresGenerationOfCorrelationId(hasCorrelationIdHeader, cid))
            {
                correlationId = GenerateCorrelationId(context.TraceIdentifier);
            }

            if (!string.IsNullOrEmpty(correlationId))
            {
                Log.UpdatingTraceIdentifier(_logger);

                context.TraceIdentifier = correlationId;
            }

            Log.CreatingCorrelationContext(_logger);
            correlationContextFactory.Create(correlationId, CorrelationIdHeaderName);

            if (!string.IsNullOrEmpty(correlationId))
            {
                // apply the correlation ID to the response header for client side tracking
                context.Response.OnStarting(() =>
                {
                    if (!context.Response.Headers.ContainsKey(CorrelationIdHeaderName))
                    {
                        Log.WritingCorrelationIdResponseHeader(_logger, CorrelationIdHeaderName, correlationId);
                        context.Response.Headers.Add(CorrelationIdHeaderName, correlationId);
                    }

                    return Task.CompletedTask;
                });
            }

            Log.CorrelationIdProcessingEnd(_logger, correlationId);
            await _next(context);

            Log.DisposingCorrelationContext(_logger);
            correlationContextFactory.Dispose();
        }

        private static bool RequiresGenerationOfCorrelationId(bool idInHeader, StringValues idFromHeader) => !idInHeader || StringValues.IsNullOrEmpty(idFromHeader);

        private StringValues GenerateCorrelationId(string traceIdentifier) => string.IsNullOrEmpty(traceIdentifier) ? Guid.NewGuid().ToString() : traceIdentifier;

        internal static class EventIds
        {
            public static readonly EventId CorrelationIdProcessingBegin = new EventId(100, "CorrelationIdProcessingBegin");
            public static readonly EventId CorrelationIdProcessingEnd = new EventId(101, "CorrelationIdProcessingEnd");

            public static readonly EventId MissingCorrelationIdProvider = new EventId(103, "MissingCorrelationIdProvider");
            public static readonly EventId EnforcedCorrelationIdHeaderMissing = new EventId(104, "EnforcedCorrelationIdHeaderMissing");
            public static readonly EventId FoundCorrelationIdHeader = new EventId(105, "EnforcedCorrelationIdHeaderMissing");
            public static readonly EventId MissingCorrelationIdHeader = new EventId(106, "MissingCorrelationIdHeader");

            public static readonly EventId GeneratedHeaderUsingGeneratorFunction = new EventId(107, "GeneratedHeaderUsingGeneratorFunction");
            public static readonly EventId GeneratedHeaderUsingProvider = new EventId(108, "GeneratedHeaderUsingProvider");

            public static readonly EventId UpdatingTraceIdentifier = new EventId(109, "UpdatingTraceIdentifier");
            public static readonly EventId CreatingCorrelationContext = new EventId(110, "CreatingCorrelationContext");
            public static readonly EventId DisposingCorrelationContext = new EventId(111, "DisposingCorrelationContext");
            public static readonly EventId WritingCorrelationIdResponseHeader = new EventId(112, "WritingCorrelationIdResponseHeader");
        }

        private static class Log
        {
            private static readonly Action<ILogger, Exception> _correlationIdProcessingBegin = LoggerMessage.Define(
                LogLevel.Debug,
                EventIds.CorrelationIdProcessingBegin,
                "Running correlation ID processing");

            private static readonly Action<ILogger, string, Exception> _correlationIdProcessingEnd = LoggerMessage.Define<string>(
                LogLevel.Debug,
                EventIds.CorrelationIdProcessingEnd,
                "Correlation ID processing was completed with a final correlation ID {CorrelationId}");

            private static readonly Action<ILogger, Exception> _missingCorrelationIdProvider = LoggerMessage.Define(
                LogLevel.Error,
                EventIds.MissingCorrelationIdProvider,
                "Correlation ID middleware was called when no ICorrelationIdProvider had been configured");

            private static readonly Action<ILogger, Exception> _enforcedCorrelationIdHeaderMissing = LoggerMessage.Define(
                LogLevel.Warning,
                EventIds.EnforcedCorrelationIdHeaderMissing,
                "Correlation ID header is enforced but no Correlation ID was not found in the request headers");

            private static readonly Action<ILogger, string, Exception> _foundCorrelationIdHeader = LoggerMessage.Define<string>(
                LogLevel.Information,
                EventIds.FoundCorrelationIdHeader,
                "Correlation ID {CorrelationId} was found in the request headers");

            private static readonly Action<ILogger, Exception> _missingCorrelationIdHeader = LoggerMessage.Define(
                LogLevel.Information,
                EventIds.MissingCorrelationIdHeader,
                "No correlation ID was found in the request headers");

            private static readonly Action<ILogger, string, Exception> _generatedHeaderUsingGeneratorFunction = LoggerMessage.Define<string>(
                LogLevel.Debug,
                EventIds.GeneratedHeaderUsingGeneratorFunction,
                "Generated a correlation ID {CorrelationId} using the configured generator function");

            private static readonly Action<ILogger, string, Type, Exception> _generatedHeaderUsingProvider = LoggerMessage.Define<string, Type>(
                LogLevel.Debug,
                EventIds.GeneratedHeaderUsingProvider,
                "Generated a correlation ID {CorrelationId} using the {Type} provider");

            private static readonly Action<ILogger, Exception> _updatingTraceIdentifier = LoggerMessage.Define(
                LogLevel.Debug,
                EventIds.UpdatingTraceIdentifier,
                "Updating the TraceIdentifier value on the HttpContext");

            private static readonly Action<ILogger, Exception> _creatingCorrelationContext = LoggerMessage.Define(
                LogLevel.Debug,
                EventIds.CreatingCorrelationContext,
                "Creating the correlation context for this request");

            private static readonly Action<ILogger, Exception> _disposingCorrelationContext = LoggerMessage.Define(
                LogLevel.Debug,
                EventIds.DisposingCorrelationContext,
                "Disposing the correlation context for this request");

            private static readonly Action<ILogger, string, string, Exception> _writingCorrelationIdResponseHeader = LoggerMessage.Define<string, string>(
                LogLevel.Debug,
                EventIds.WritingCorrelationIdResponseHeader,
                "Writing correlation ID response header {ResponseHeader} with value {CorrelationId}");

            public static void CorrelationIdProcessingBegin(ILogger logger)
            {
                if (logger.IsEnabled(LogLevel.Debug))
                    _correlationIdProcessingBegin(logger, null);
            }

            public static void CorrelationIdProcessingEnd(ILogger logger, string correlationId)
            {
                if (logger.IsEnabled(LogLevel.Debug))
                    _correlationIdProcessingEnd(logger, correlationId, null);
            }

            public static void MissingCorrelationIdProvider(ILogger logger) => _missingCorrelationIdProvider(logger, null);

            public static void EnforcedCorrelationIdHeaderMissing(ILogger logger) => _enforcedCorrelationIdHeaderMissing(logger, null);

            public static void FoundCorrelationIdHeader(ILogger logger, string correlationId) => _foundCorrelationIdHeader(logger, correlationId, null);

            public static void MissingCorrelationIdHeader(ILogger logger) => _missingCorrelationIdHeader(logger, null);

            public static void GeneratedHeaderUsingGeneratorFunction(ILogger logger, string correlationId) => _generatedHeaderUsingGeneratorFunction(logger, correlationId, null);

            public static void GeneratedHeaderUsingProvider(ILogger logger, string correlationId, Type type) => _generatedHeaderUsingProvider(logger, correlationId, type, null);

            public static void UpdatingTraceIdentifier(ILogger logger) => _updatingTraceIdentifier(logger, null);

            public static void CreatingCorrelationContext(ILogger logger) => _creatingCorrelationContext(logger, null);

            public static void DisposingCorrelationContext(ILogger logger) => _disposingCorrelationContext(logger, null);

            public static void WritingCorrelationIdResponseHeader(ILogger logger, string headerName, string correlationId) => _writingCorrelationIdResponseHeader(logger, headerName, correlationId, null);
        }
    }
}