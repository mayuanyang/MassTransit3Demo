using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Pipeline;
using Serilog;
using Serilog.Events;

namespace MassTransit3Demo.Core.MiddlewareExtensions.PerformanceLogger
{
    public class PerformanceLoggerFilter<T> :
    IFilter<T>
    where T : class, PipeContext
    {
        private readonly ILogger _logger;

        public PerformanceLoggerFilter(ILogger logger)
        {
            _logger = logger;
        }

        public void Probe(ProbeContext context)
        {
            
        }

        public async Task Send(T context, IPipe<T> next)
        {
            using (_logger.BeginTimedOperation("Time for handling message", null, LogEventLevel.Debug))
            {
                Console.WriteLine($"Middleware Started: {typeof(PerformanceLoggerExtension).Name}");
                await next.Send(context);
                Console.WriteLine($"Middleware Finished: {typeof(PerformanceLoggerExtension).Name}");
            }
        }
    }
}
