using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit3Demo.Messages.Commands;

namespace MassTransit3Demo.Core.Consumers
{
    public class PrintToConsoleCommandConsumer : IConsumer<PrintToConsoleCommand>
    {
        public Task Consume(ConsumeContext<PrintToConsoleCommand> context)
        {
            Console.WriteLine(context.Message.Text);
            return Task.FromResult(1);
        }

        
    }
}
