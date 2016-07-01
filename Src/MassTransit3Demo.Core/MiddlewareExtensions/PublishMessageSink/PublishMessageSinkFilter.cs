using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Pipeline;
using MassTransit3Demo.Messages.Events;

namespace MassTransit3Demo.Core.MiddlewareExtensions.PublishMessageSink
{
    public class PublishMessageSinkFilter<T> :
        IFilter<T>
        where T : class, SendContext
    {
        public async Task Send(T context, IPipe<T> next)
        {
            var sendContext = context as SendContext<IEvent>;
            if(sendContext != null)
                Console.WriteLine($"I am saving the message {sendContext.Message.GetType().Name} to the repo");

            await next.Send(context);

        }

        public void Probe(ProbeContext context)
        {

        }
    }
}