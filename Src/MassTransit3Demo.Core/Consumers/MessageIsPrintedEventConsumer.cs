using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit3Demo.Messages.Events;

namespace MassTransit3Demo.Core.Consumers
{
    public class MessageIsPrintedEventConsumer : IConsumer<MessageIsPrintedEvent>
    {
        public Task Consume(ConsumeContext<MessageIsPrintedEvent> context)
        {
            Console.WriteLine("Message is printed");
            return Task.FromResult(1);
        }
    }
}
