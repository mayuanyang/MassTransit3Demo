using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Pipeline;
using MassTransit3Demo.Messages.Events;

namespace MassTransit3Demo.Core.MiddlewareExtensions.PublishMessageSink
{
    public class EventStoreFilter<T> :
        IFilter<T>
        where T : class, SendContext
    {
        public async Task Send(T context, IPipe<T> next)
        {
            var sendContext = context as SendContext<IEvent>;
            if(sendContext != null)
                Console.WriteLine($"Message {sendContext.Message.GetType().Name} has been saved to EventStore");

            await next.Send(context);

        }

        public void Probe(ProbeContext context)
        {

        }
    }
}