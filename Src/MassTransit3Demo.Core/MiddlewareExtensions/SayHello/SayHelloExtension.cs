using MassTransit;
using Serilog;

namespace MassTransit3Demo.Core.MiddlewareExtensions.SayHello
{
    public static class SayHelloExtension
    {
        public static void UseSayHello<T>(this IPipeConfigurator<T> configurator)
            where T : class, PipeContext
        {
            configurator.AddPipeSpecification(new SayHelloPipeSpecification<T>());
        }
    }
}
