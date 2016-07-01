using MassTransit;
using MassTransit.PipeConfigurators;


namespace MassTransit3Demo.Core.MiddlewareExtensions.PublishMessageSink
{
    public static class PublishMessageSinkExtension 
    {
        public static void UsePublishMessageSink<T>(this ISendPipeConfigurator configurator)
            where T : class, SendContext
        {
            configurator.AddPipeSpecification((IPipeSpecification<SendContext>)new PublishMessageSinkPipeSpecification<T>());
        }
    }
}
