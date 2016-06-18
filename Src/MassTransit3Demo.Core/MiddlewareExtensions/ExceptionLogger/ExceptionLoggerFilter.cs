using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Pipeline;
using Serilog;

namespace MassTransit3Demo.Core.MiddlewareExtensions.ExceptionLogger
{
    public class ExceptionLoggerFilter<T> :
    IFilter<T>
    where T : class, PipeContext
    {
        private readonly ILogger _logger;
        long _exceptionCount;
        long _successCount;
        long _attemptCount;

        public ExceptionLoggerFilter(ILogger logger)
        {
            _logger = logger;
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("exceptionLogger");
            scope.Add("attempted", _attemptCount);
            scope.Add("succeeded", _successCount);
            scope.Add("faulted", _exceptionCount);
            
        }

        public async Task Send(T context, IPipe<T> next)
        {
            try
            {
                
                Interlocked.Increment(ref _attemptCount);
                Console.WriteLine($"Middleware Started: {typeof(ExceptionLoggerExtension).Name}");
                await next.Send(context);
                Console.WriteLine($"Middleware Finished: {typeof(ExceptionLoggerExtension).Name}");
                Interlocked.Increment(ref _successCount);
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref _exceptionCount);

                _logger.Error(ex, $"An exception occurred: {ex.Message}");

                // propagate the exception up the call stack
                throw;
            }
        }
    }


}
