using MassTransit;
using MassTransit.PipeConfigurators;


namespace MassTransit3Demo.Core.MiddlewareExtensions.PublishMessageSink
{
    public static class EventStoreExtension 
    {
        public static void UseEventStore<T>(this ISendPipeConfigurator configurator)
            where T : class, SendContext
        {
            configurator.AddPipeSpecification((IPipeSpecification<SendContext>)new EventStorePipeSpecification<T>());
        }
    }
}
