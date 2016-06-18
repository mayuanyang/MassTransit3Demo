using MassTransit;
using Serilog;

namespace MassTransit3Demo.Core.MiddlewareExtensions.PerformanceLogger
{
    public static class PerformanceLoggerExtension
    {
        public static void UsePerformanceLogger<T>(this IPipeConfigurator<T> configurator, ILogger logger)
            where T : class, PipeContext
        {
            configurator.AddPipeSpecification(new PerformanceLoggerPipeSpecification<T>(logger));
        }
    }
}
