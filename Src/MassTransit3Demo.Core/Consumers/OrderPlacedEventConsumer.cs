using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit3Demo.Messages.Events;

namespace MassTransit3Demo.Core.Consumers
{
    public class OrderPlacedEventConsumer : IConsumer<IOrderPlacedEvent>
    {
        public Task Consume(ConsumeContext<IOrderPlacedEvent> context)
        {
            Console.WriteLine($"The order amount is: {context.Message.OrderAmount}");

            ConsumeContext<IOrderPlacedEventV2> v2Context;
            if (context.TryGetMessage(out v2Context))
            {
                Console.WriteLine($"The order amount is: {context.Message.OrderAmount} and the postage is {v2Context.Message.Postage}");
            }

            return Task.FromResult(1);
        }
    }
}
