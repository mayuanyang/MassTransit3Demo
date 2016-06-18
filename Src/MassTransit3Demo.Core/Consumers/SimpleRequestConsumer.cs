using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit3Demo.Messages;

namespace MassTransit3Demo.Core.Consumers
{
    class SimpleRequestConsumer : IConsumer<SimpleRequest>
    {
        public async Task Consume(ConsumeContext<SimpleRequest> context)
        {
            Console.WriteLine($"Handling request: {context.Message.Name}");
            var response = new SimpleResonse("Hello " + context.Message.Name);
            await context.RespondAsync(response);
        }
    }
}
