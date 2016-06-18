using MassTransit;
using Serilog;

namespace MassTransit3Demo.Core.MiddlewareExtensions.ExceptionLogger
{
    public static class ExceptionLoggerExtension
    {
        
        public static void UseExceptionLogger<T>(this IPipeConfigurator<T> configurator, ILogger logger)
            where T : class, PipeContext
        {
            configurator.AddPipeSpecification(new ExceptionLoggerSpecification<T>(logger));
        }
    }
}