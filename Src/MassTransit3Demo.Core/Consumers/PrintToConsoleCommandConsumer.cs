using System;
using System.Data;
using System.Threading.Tasks;
using MassTransit;
using MassTransit3Demo.Messages.Commands;
using MassTransit3Demo.Messages.Events;

namespace MassTransit3Demo.Core.Consumers
{
    public class PrintToConsoleCommandConsumer : IConsumer<PrintToConsoleCommand>
    {
 
        public async Task Consume(ConsumeContext<PrintToConsoleCommand> context)
        {
            try
            {
                Console.WriteLine(context.Message.Text);
                await context.Publish(new MessageIsPrintedEvent(context.Message.Text));

            }
            catch (DBConcurrencyException)
            {
                await context.Redeliver(TimeSpan.FromSeconds(20));
            }
        }

        
    }
}
