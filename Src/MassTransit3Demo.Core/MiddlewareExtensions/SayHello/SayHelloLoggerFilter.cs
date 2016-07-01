using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Pipeline;
using Serilog;

namespace MassTransit3Demo.Core.MiddlewareExtensions.SayHello
{
    public class SayHelloFilter<T> :
    IFilter<T>
    where T : class, PipeContext
    {
        

        public void Probe(ProbeContext context)
        {
            
        }

        public async Task Send(T context, IPipe<T> next)
        {
            Console.WriteLine();
            Console.WriteLine($"Middleware Started: {typeof(SayHelloExtension).Name}");
            await next.Send(context);
            Console.WriteLine($"Middleware Finished: {typeof(SayHelloExtension).Name}");
            
        }
    }
}
