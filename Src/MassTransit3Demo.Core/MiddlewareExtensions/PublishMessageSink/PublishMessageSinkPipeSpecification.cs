using System.Collections.Generic;
using System.Linq;
using MassTransit;
using MassTransit.Configurators;
using MassTransit.PipeBuilders;
using MassTransit.PipeConfigurators;

namespace MassTransit3Demo.Core.MiddlewareExtensions.PublishMessageSink
{
    public class PublishMessageSinkPipeSpecification<T> :
        IPipeSpecification<T>
        where T : class, SendContext
    {
        public void Apply(IPipeBuilder<T> builder)
        {
            builder.AddFilter(new PublishMessageSinkFilter<T>());
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return Enumerable.Empty<ValidationResult>();
        }
    }
}